using System;

namespace Nooshka.Impl
{
    public abstract class ServiceBuilder : IServiceBuilder
    {
        protected ServiceBuilder(ServiceDefinition definition)
        {
            Definition = definition;
        }

        public ServiceDefinition Definition { get; }

        public bool PreconditionMet(ServiceRequest request) =>
            Definition.Precondition(request);

        public object GetService(ServiceRequest request)
        {
            if (!PreconditionMet(request)) return null;
            var servicingContainer =
                Definition.Lifecycle.GetServicingContainer(request);
            var cache = servicingContainer.ServiceCache;
            var tracker = servicingContainer.ServiceTracker;
            var service = cache.Retrieve(Definition);
            if (service == null) {
                service = Resolve(request);
                cache.Store(Definition, service);
                if (Definition.Lifecycle.IsTracked && service is IDisposable disposable) {
                    tracker.Track(disposable);
                }
            }

            return service;
        }

        protected abstract object Resolve(ServiceRequest request);
    }
}
