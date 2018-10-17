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
            var serviceTracker = Definition.Lifecycle.GetTracker(request);
            var serviceCache = Definition.Lifecycle.GetCache(request);
            var service = serviceCache?.Retrieve(Definition);
            if (service == null) {
                service = Resolve(request);
                serviceCache?.Store(Definition, service);
                if (service is IDisposable disposable) {
                    serviceTracker?.Track(disposable);
                }
            }

            return service;
        }

        protected abstract object Resolve(ServiceRequest request);
    }
}
