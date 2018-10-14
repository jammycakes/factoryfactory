using System;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Lifecycles;

namespace Nooshka.Registration
{
    /// <summary>
    ///  Represents a basic service registration in the container.
    /// </summary>
    public class ServiceRegistration : IRegistration
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
        public Lifecycle Lifecycle { get; }

        /// <summary>
        ///  Registers a service to be resolved as a concrete type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifecycle"></param>
        /// <param name="precondition"></param>
        public ServiceRegistration(Type serviceType,
            Type implementationType,
            Lifecycle lifecycle,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ImplementationFactory = null;
            Lifecycle = lifecycle;
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
            Lifecycle = new TransientLifecycle();
            Precondition = precondition ?? (sr => true);
        }

        /// <summary>
        ///  Registers a service to be provided by an implementation factory.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="lifecycle"></param>
        /// <param name="precondition"></param>
        public ServiceRegistration(Type serviceType,
            Func<ServiceRequest, object> implementationFactory,
            Lifecycle lifecycle,
            Func<ServiceRequest, bool> precondition = null)
        {
            ServiceType = serviceType;
            ImplementationFactory = implementationFactory;
            ImplementationType = null;
            Lifecycle = lifecycle;
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
                    Lifecycle = new TransientLifecycle();
                    break;
                case ServiceLifetime.Scoped:
                    Lifecycle = new ScopedLifecycle();
                    break;
                case ServiceLifetime.Singleton:
                    Lifecycle = new SingletonLifecycle();
                    break;
            }

            Precondition = precondition ?? (sr => true);
        }
    }
}
