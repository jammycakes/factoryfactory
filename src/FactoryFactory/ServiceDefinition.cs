using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FactoryFactory.Util;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class ServiceDefinition : IServiceDefinition
    {
        private readonly object _identity = new object();

        /* ====== Constructors ====== */

        /// <summary>
        ///  Creates a new default instance of the <see cref="ServiceDefinition"/>
        ///  class.
        /// </summary>
        public ServiceDefinition(Type serviceType,
            Expression<Func<ServiceRequest, object>> implementationFactory = null,
            Type implementationType = null,
            object implementationInstance = null,
            ILifecycle lifecycle = null,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            Precondition = precondition;

            ImplementationFactory = implementationFactory;
            ImplementationInstance = implementationInstance;
            ImplementationType = implementationType ?? (implementationFactory == null ? implementationType : null);
            Lifecycle = lifecycle ?? FactoryFactory.Lifecycle.Default;

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
                Lifecycle = FactoryFactory.Lifecycle.Get(descriptor.Lifetime);
            }
            else if (descriptor.ImplementationInstance != null) {
                var instance = descriptor.ImplementationInstance;
                ImplementationInstance = instance;
                Lifecycle = FactoryFactory.Lifecycle.Untracked;
            }
            else if (descriptor.ImplementationFactory != null) {
                var factory = descriptor.ImplementationFactory;
                ImplementationFactory = sr => factory(sr.Container);
                Lifecycle = FactoryFactory.Lifecycle.Get(descriptor.Lifetime);
            }
            else {
                throw new IoCException
                    ("Invalid descriptor: neither a service nor a service factory has been set.");
            }

            Precondition = precondition;
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
        ///  The instance that provides the service, or null if the
        ///  service is specified by type or factory expression.
        /// </summary>
        public object ImplementationInstance { get; }

        /// <summary>
        ///  The type that will be constructed which implements the service type
        ///  specified in ServiceType.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        ///  The <see cref="FactoryFactory.Lifecycle"/> implementation that tells the
        ///  container where to resolve and when to release dependencies.
        /// </summary>
        public ILifecycle Lifecycle { get; }

        int IServiceDefinition.Priority => ServiceType.IsGenericTypeDefinition ? 1000 : 2000;

        /// <summary>
        ///  Gets a value indicating whether this definition was created for a
        ///  generic type definition.
        /// </summary>
        public bool IsForOpenGeneric { get; private set; }


        private void Validate()
        {
            // ServiceType must be specified.
            if (ServiceType == null) {
                throw new IoCException("No service type was specified.");
            }

            // Implementation type or implementation expression or implementation
            // instance must be specified, but no more than one of the above.
            var count = new object[]
                    {ImplementationType, ImplementationFactory, ImplementationInstance}
                .Count(x => x != null);

            if (count == 0) {
                throw new IoCException
                    ($"No implementation was specified for type {ServiceType.FullName}.");
            }

            // Implementation type and implementation expression can not both be specified
            if (count > 1) {
                throw new IoCException
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

            if (ImplementationInstance != null) {
                actualImplementationType = ImplementationInstance.GetType();
            }

            // Implementation must not be a value type.
            if (actualImplementationType.IsValueType) {
                throw new IoCException
                    ($"Type {ServiceType.FullName} has been specified to be " +
                     $"implemented by type {actualImplementationType.FullName}, " +
                     $"which is a value type. Value types are not supported as " +
                     $"registered services.");
            }

            // Implementations by type must be concrete classes.
            if (ImplementationType != null && ImplementationType.IsAbstract) {
                throw new IoCException
                    ($"Type {ServiceType.FullName} has been specified to be " +
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
                throw new IoCException
                    ($"Open generic type {name} must be implemented by type, " +
                     $"not by instance or expression.");
            }

            var impl = ImplementationType.FullName;
            // Open generic implementations must also be open generics.
            if (!ImplementationType.IsGenericTypeDefinition
                ) {
                throw new IoCException
                    ($"{impl} is not a valid implementation for {name} as it " +
                     $"is not an open generic.");
            }

            // Open generic implementations must either implement the service interface...
            if (ImplementationType.InheritsOrImplements(ServiceType)) {
                return;
            }

            throw new IoCException
                ($"Implementation type {ImplementationType.FullName} does not " +
                 $"implement or derive from service type {ServiceType.FullName}.");
        }

        private void ValidateClosedType(Type impl)
        {
            // Implementing type must be assignable from service type.
            if (!ServiceType.IsAssignableFrom(impl)) {
                throw new IoCException
                    ($"Type {impl.FullName} can not be assigned " +
                     $"to a value of type {ServiceType.FullName}.");
            }
        }

        IEnumerable<Type> IServiceDefinition.GetTypes(Type requestedType)
        {
            if (ImplementationType != null) {
                if (requestedType == ServiceType) {
                    yield return ImplementationType;
                }
                else if (requestedType.IsGenericType && ServiceType.IsGenericTypeDefinition) {
                    var openedRequest = requestedType.GetGenericTypeDefinition();
                    if (openedRequest == ServiceType) {
                        Type closedRequest;
                        try {
                            closedRequest = ImplementationType.MakeGenericType
                                (requestedType.GenericTypeArguments);
                        }
                        catch {
                            /*
                             * Ugly Pokémon exception handling because it's the
                             * only reasonable way to determine whether a
                             * generic type constraint works.
                             */
                            closedRequest = null;
                        }

                        if (closedRequest != null) {
                            yield return closedRequest;
                        }
                    }
                }
            }
        }

        IEnumerable<object> IServiceDefinition.GetInstances(Type requestedType)
        {
            if (ImplementationInstance != null && requestedType == ServiceType) {
                yield return ImplementationInstance;
            }
        }

        IEnumerable<Expression<Func<ServiceRequest, object>>> IServiceDefinition.GetExpressions(Type requestedType)
        {
            if (ImplementationFactory != null && requestedType == ServiceType) {
                yield return ImplementationFactory;
            }
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
