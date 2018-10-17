using System;

namespace Nooshka.Impl
{
    public abstract class ResolverBase : IResolver
    {
        protected ResolverBase(ServiceDefinition serviceDefinition)
        {
            ServiceDefinition = serviceDefinition;
        }

        protected ServiceDefinition ServiceDefinition { get; }

        public bool PreconditionMet(ServiceRequest request) =>
            ServiceDefinition.Precondition(request);

        public object GetService(ServiceRequest request)
        {
            if (!PreconditionMet(request)) return null;
            var servicingContainer =
                ServiceDefinition.Lifecycle.GetServicingContainer(request);
            var lifecycleManager = servicingContainer.ServiceCache;
            var service = lifecycleManager.Retrieve(ServiceDefinition);
            if (service == null) {
                service = Resolve(request);
                lifecycleManager.Store(ServiceDefinition, service);
                if (ServiceDefinition.Lifecycle.IsTracked && service is IDisposable disposable) {
                    lifecycleManager.Track(disposable);
                }
            }

            return service;
        }

        protected abstract object Resolve(ServiceRequest request);
    }
}
