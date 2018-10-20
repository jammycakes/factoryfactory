using System;

namespace FactoryFactory.Impl
{
    public class ServiceResolver : IServiceResolver
    {
        private readonly IServiceBuilder _builder;

        public ServiceResolver(ServiceDefinition definition, IServiceBuilder builder)
        {
            _builder = builder;
            Definition = definition;
            IsOpenGeneric = definition.IsForOpenGeneric;
        }

        public ServiceDefinition Definition { get; }

        public bool IsOpenGeneric { get; }

        public bool PreconditionMet(ServiceRequest request) =>
            Definition.Precondition(request);

        public object GetService(ServiceRequest request)
        {
            if (!PreconditionMet(request)) return null;
            var serviceTracker = Definition.Lifecycle.GetTracker(request);
            var serviceCache = Definition.Lifecycle.GetCache(request);
            var service = serviceCache?.Retrieve(Definition);
            if (service == null) {
                service = _builder.GetService(request);
                serviceCache?.Store(Definition, service);
                if (service is IDisposable disposable) {
                    serviceTracker?.Track(disposable);
                }
            }

            return service;
        }
    }
}
