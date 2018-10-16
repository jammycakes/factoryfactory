using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nooshka.Impl
{
    public class Registration
    {
        private bool _locked = false;

        private Type _serviceType;
        private Func<ServiceRequest, bool> _precondition = req => true;
        private Func<ServiceRequest, object> _implementationFactory;
        private Type _implementationType;
        private Lifecycle _lifecycle = Lifecycle.Default;

        /* ====== Constructors ====== */

        /// <summary>
        ///  Creates a new default instance of the <see cref="Registration"/>
        ///  class.
        /// </summary>
        public Registration(Type serviceType)
        {
            _serviceType = serviceType;
            _locked = false;
        }

        /// <summary>
        ///  Registers a service to be provided based on a
        ///  <see cref="ServiceDescriptor"/> instance.
        /// </summary>
        /// <param name="serviceDescriptor"></param>
        public Registration(ServiceDescriptor descriptor,
            Func<ServiceRequest, bool> precondition = null)
        {
            _serviceType = descriptor.ServiceType;
            if (descriptor.ImplementationType != null) {
                _implementationType = descriptor.ImplementationType;
            }
            else if (descriptor.ImplementationInstance != null) {
                var instance = descriptor.ImplementationInstance;
                _implementationFactory = sr => instance;
            }
            else if (descriptor.ImplementationFactory != null) {
                var factory = descriptor.ImplementationFactory;
                _implementationFactory = sr => factory(sr.Container);
            }
            else {
                throw new ConfigurationException
                    ("Invalid descriptor: neither a service nor a service factory has been set.");
            }

            _lifecycle = Lifecycle.Get(descriptor.Lifetime);
            _precondition = precondition ?? (sr => true);
            _locked = true;
        }

        /* ====== Lock on read ====== */

        private T Lock<T>(T obj)
        {
            _locked = true;
            return obj;
        }

        private T AssertUnlocked<T>(T obj)
        {
            if (_locked) {
                throw new InvalidOperationException
                    ("You can not change a service registration after it has been accessed.");
            }

            return obj;
        }

        /* ====== Properties ====== */

        /// <summary>
        ///  The type of service being registered.
        /// </summary>
        public Type ServiceType => _serviceType;

        /// <summary>
        ///  The precondition for this service registration to be activated.
        /// </summary>
        public Func<ServiceRequest, bool> Precondition {
            get => Lock(_precondition);
            set => _precondition = AssertUnlocked(value);
        }


        /// <summary>
        ///  The factory method that instantiates the service, or null if a
        ///  specific type is specified in the ServiceResolution property.
        /// </summary>
        public Func<ServiceRequest, object> ImplementationFactory {
            get => Lock(_implementationFactory);
            set => _implementationFactory = AssertUnlocked(value);
        }

        /// <summary>
        ///  The type that will be constructed which implements the service type
        ///  specified in ServiceType.
        /// </summary>
        public Type ImplementationType {
            get => Lock(_implementationType);
            set => _implementationType = AssertUnlocked(value);
        }

        /// <summary>
        ///  The <see cref="Nooshka.Lifecycle"/> implementation that tells the
        ///  container where to resolve and when to release dependencies.
        /// </summary>
        public Lifecycle Lifecycle {
            get => Lock(_lifecycle);
            set => _lifecycle = AssertUnlocked(value);
        }
    }
}
