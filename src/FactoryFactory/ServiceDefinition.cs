using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class ServiceDefinition
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
            Lifecycle lifecycle = null,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            Precondition = precondition ?? (req => true);
            ImplementationFactory = implementationFactory;
            ImplementationType = implementationType ?? (implementationFactory == null ? implementationType : null);
            Lifecycle = lifecycle ?? Lifecycle.Default;
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
                throw new ConfigurationException
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
    }
}
