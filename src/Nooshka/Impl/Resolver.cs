using System;

namespace Nooshka.Impl
{
    public abstract class ResolverBase : IResolver
    {
        protected ResolverBase(ServiceDefinition definition)
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
            var service = cache.Retrieve(Definition);
            if (service == null) {
                service = Resolve(request);
                cache.Store(Definition, service);
                if (Definition.Lifecycle.IsTracked && service is IDisposable disposable) {
                    cache.Track(disposable);
                }
            }

            return service;
        }

        protected abstract object Resolve(ServiceRequest request);
    }
}
