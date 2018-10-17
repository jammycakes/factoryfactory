using System;

namespace Nooshka.Impl
{
    public abstract class Resolver
    {
        public Resolver(ServiceDefinition serviceDefinition)
        {
            ServiceDefinition = serviceDefinition;
        }

        public ServiceDefinition ServiceDefinition { get; }

        public bool PreconditionMet(ServiceRequest request) =>
            ServiceDefinition.Precondition(request);

        public object GetService(ServiceRequest request)
        {
            if (!PreconditionMet(request)) return null;
            var servicingContainer =
                ServiceDefinition.Lifecycle.GetServicingContainer(request);
            var lifecycleManager = servicingContainer.LifecycleManager;
            var service = lifecycleManager.GetExisting(ServiceDefinition);
            if (service == null) {
                service = Resolve(request);
                lifecycleManager.Cache(ServiceDefinition, service);
                if (ServiceDefinition.Lifecycle.IsTracked && service is IDisposable disposable) {
                    lifecycleManager.Track(disposable);
                }
            }

            return service;
        }

        protected abstract object Resolve(ServiceRequest request);
    }
}
