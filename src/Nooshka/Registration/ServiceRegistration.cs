using System;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Lifecycle;

namespace Nooshka
{
    /// <summary>
    ///  Represents a basic service registration in the container.
    /// </summary>
    public class ServiceRegistration
    {
        /// <summary>
        ///  The type of service being registered.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///  The precondition for this service registration to be activated.
        /// </summary>
        public Func<ServiceRequest, bool> Precondition { get; }

        /// <summary>
        ///  The factory method that instantiates the service, or null if a
        ///  specific type is specified in the ServiceResolution property.
        /// </summary>
        public Func<ServiceRequest, object> ImplementationFactory { get; }

        /// <summary>
        ///  The type that will be constructed which implements the service type
        ///  specified in ServiceType.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        ///
        /// </summary>
        public ILifetime Lifetime { get; }

        /// <summary>
        ///  Registers a service to be resolved as a concrete type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifetime"></param>
        /// <param name="precondition"></param>
        public ServiceRegistration(Type serviceType,
            Type implementationType,
            ILifetime lifetime,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ImplementationFactory = null;
            Lifetime = lifetime;
            Precondition = precondition ?? (sr => true);
        }

        /// <summary>
        ///  Registers a service to be provided by a specific object.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementation"></param>
        /// <param name="precondition"></param>
        public ServiceRegistration(Type serviceType,
            object implementation,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            ImplementationFactory = sr => implementation;
            ImplementationType = null;
            Lifetime = new TransientLifetime();
            Precondition = precondition ?? (sr => true);
        }

        /// <summary>
        ///  Registers a service to be provided by an implementation factory.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="lifetime"></param>
        /// <param name="precondition"></param>
        public ServiceRegistration(Type serviceType,
            Func<ServiceRequest, object> implementationFactory,
            ILifetime lifetime,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            ImplementationFactory = implementationFactory;
            ImplementationType = null;
            Lifetime = lifetime;
            Precondition = precondition ?? (sr => true);
        }

        /// <summary>
        ///  Registers a service to be provided based on a
        ///  <see cref="ServiceDescriptor"/> instance.
        /// </summary>
        /// <param name="serviceDescriptor"></param>
        public ServiceRegistration(ServiceDescriptor descriptor,
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

            switch (descriptor.Lifetime) {
                case ServiceLifetime.Transient:
                    Lifetime = new TransientLifetime();
                    break;
                case ServiceLifetime.Scoped:
                    Lifetime = new ScopedLifetime();
                    break;
                case ServiceLifetime.Singleton:
                    Lifetime = new SingletonLifetime();
                    break;
            }

            Precondition = precondition ?? (sr => true);
        }
    }
}
