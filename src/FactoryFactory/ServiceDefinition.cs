using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class ServiceDefinition
    {
        private readonly object _identity = new object();
        private IDictionary<Type, ServiceDefinition> _genericDefinitions
            = new ConcurrentDictionary<Type, ServiceDefinition>();

        /* ====== Constructors ====== */

        /// <summary>
        ///  Creates a new default instance of the <see cref="ServiceDefinition"/>
        ///  class.
        /// </summary>
        public ServiceDefinition(Type serviceType,
            Expression<Func<ServiceRequest, object>> implementationFactory = null,
            Type implementationType = null,
            Lifecycle lifecycle = null,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            Precondition = precondition ?? (req => true);
            ImplementationFactory = implementationFactory;
            ImplementationType = implementationType ?? (implementationFactory == null ? implementationType : null);
            Lifecycle = lifecycle ?? Lifecycle.Default;

            Validate();
        }

        /// <summary>
        ///  Registers a service to be provided based on a
        ///  <see cref="ServiceDescriptor"/> instance.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="precondition"></param>
        public ServiceDefinition(ServiceDescriptor descriptor,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = descriptor.ServiceType;
            if (descriptor.ImplementationType != null) {
                ImplementationType = descriptor.ImplementationType;
            }
            else if (descriptor.ImplementationInstance != null) {
                var instance = descriptor.ImplementationInstance;
                ImplementationFactory = sr => instance;
            }
            else if (descriptor.ImplementationFactory != null) {
                var factory = descriptor.ImplementationFactory;
                ImplementationFactory = sr => factory(sr.Container);
            }
            else {
                throw new ServiceDefinitionException
                    ("Invalid descriptor: neither a service nor a service factory has been set.");
            }

            Lifecycle = Lifecycle.Get(descriptor.Lifetime);
            Precondition = precondition ?? (sr => true);
        }


        /* ====== Properties ====== */

        /// <summary>
        ///  The type of service being registered.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///  The precondition for this service definition to be activated.
        /// </summary>
        public Func<ServiceRequest, bool> Precondition { get;  }


        /// <summary>
        ///  The factory method that instantiates the service, or null if a
        ///  specific type is specified in the ServiceResolution property.
        /// </summary>
        public Expression<Func<ServiceRequest, object>> ImplementationFactory { get; }

        /// <summary>
        ///  The type that will be constructed which implements the service type
        ///  specified in ServiceType.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        ///  The <see cref="FactoryFactory.Lifecycle"/> implementation that tells the
        ///  container where to resolve and when to release dependencies.
        /// </summary>
        public Lifecycle Lifecycle { get; }

        /// <summary>
        ///  Gets a value indicating whether this definition was created for a
        ///  generic type definition.
        /// </summary>
        public bool IsForOpenGeneric { get; private set; }


        private void Validate()
        {
            // ServiceType must be specified.
            if (ServiceType == null) {
                throw new ServiceDefinitionException("No service type was specified.");
            }

            // Implementation type or implementation expression must be specified.
            if (ImplementationType == null && ImplementationFactory == null) {
                throw new ServiceDefinitionException
                    ($"No implementation was specified for type {ServiceType.FullName}.");
            }

            // Implementation type and implementation expression can not both be specified
            if (ImplementationType != null && ImplementationFactory != null) {
                throw new ServiceDefinitionException
                    ("More than one implementation was specified for type " +
                     ServiceType.FullName);
            }

            var actualImplementationType = ImplementationType;

            if (ImplementationFactory != null) {
                if (ImplementationFactory.Body is UnaryExpression uex) {
                    if (uex.NodeType == ExpressionType.Convert) {
                        actualImplementationType = uex.Operand.Type;
                    }
                }
                actualImplementationType = actualImplementationType ??
                    ImplementationFactory.Body.Type;
            }

            // Implementation must not be a value type.
            if (actualImplementationType.IsValueType) {
                throw new ServiceDefinitionException
                    ($"Type {ServiceType.FullName} has been specified to be " +
                     $"implemented by type {actualImplementationType.FullName}, " +
                     $"which is a value type. Value types are not supported as " +
                     $"registered services.");
            }

            // Implementations by type must be concrete classes.
            if (ImplementationType != null && ImplementationType.IsAbstract) {
                throw new ServiceDefinitionException
                    ($"Type {ServiceType.FullName} has been specified bo be " +
                     $"implemented by type {ImplementationType.FullName}, " +
                     $"which is an interface or an abstract base class. " +
                     $"Services implemented by type must be concrete classes.");
            }

            if (ServiceType.IsGenericTypeDefinition) {
                ValidateOpenGeneric();
            }
            else {
                ValidateClosedType(actualImplementationType);
            }
        }

        private void ValidateOpenGeneric()
        {
            string name = ServiceType.FullName;

            // Open generics must be registered by type.
            if (ImplementationType == null) {
                throw new ServiceDefinitionException
                    ($"Open generic type {name} must be implemented by type, " +
                     $"not by instance or expression.");
            }

            var impl = ImplementationType.FullName;
            // Open generic implementations must also be open generics.
            if (!ImplementationType.IsGenericTypeDefinition
                ) {
                throw new ServiceDefinitionException
                    ($"{impl} is not a valid implementation for {name} as it " +
                     $"is not an open generic.");
            }

            // Open generic implementations must either implement the service interface...
            if (ImplementationType.GetInterfaces().Contains(ServiceType)) {
                return;
            }

            // Or else be derived from the service base class
            var baseClass = ImplementationType;
            while (baseClass != null) {
                if (baseClass == ServiceType) {
                    return;
                }

                baseClass = baseClass.BaseType;
            }

            throw new ServiceDefinitionException
                ($"Implementation type {ImplementationType.FullName} does not " +
                 $"implement or derive from service type {ServiceType.FullName}.");
        }

        private void ValidateClosedType(Type impl)
        {
            // Implementing type must be assignable from service type.
            if (!ServiceType.IsAssignableFrom(impl)) {
                throw new ServiceDefinitionException
                    ($"Type {impl.FullName} can not be assigned " +
                     $"to a value of type {ServiceType.FullName}.");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ServiceDefinition GetGenericDefinition(Type requestedType)
        {
            if (!requestedType.IsGenericType) return this;
            if (!ServiceType.IsGenericTypeDefinition) return this;
            if (!_genericDefinitions.TryGetValue(requestedType, out var result)) {
                result = Close(requestedType);
                _genericDefinitions.Add(requestedType, result);
            }

            return result;
        }

        /// <summary>
        ///  Closes an open generic type. You can override this to customise
        ///  how open generics are implemented.
        /// </summary>
        /// <param name="requestedType">
        ///  The concrete type that is being requested.
        /// </param>
        /// <returns>
        ///  A new <see cref="ServiceDefinition"/> instance.
        /// </returns>
        protected virtual ServiceDefinition Close(Type requestedType)
        {
            var newType =
                ServiceType.MakeGenericType(requestedType.GenericTypeArguments);
            var newImplementationType =
                ImplementationType.MakeGenericType(requestedType
                    .GenericTypeArguments);
            return new ServiceDefinition(newType,
                ImplementationFactory, newImplementationType,
                Lifecycle, Precondition)
            {
                IsForOpenGeneric = true
            };
        }


        public override bool Equals(object obj)
        {
            if (obj is ServiceDefinition reg) {
                return reg._identity == _identity;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _identity.GetHashCode();
        }

        public override string ToString() =>
            $"ServiceDefinition: {this.ServiceType.ToString()}";
    }
}
