using System;
using System.Collections.Generic;

namespace FactoryFactory.Impl
{
    public class ServiceResolver<TService> : IServiceResolver where TService : class
    {
        private readonly IServiceBuilder _builder;

        public ServiceResolver(ServiceDefinition definition, IServiceBuilder builder)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            Definition = definition;
            _builder = builder;
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
            TService service = (TService)serviceCache?.Retrieve(Definition);
            if (service == null) {
                service = (TService)_builder.GetService(request);
                foreach (var decorator in GetDecorators(request)) {
                    service = decorator.Decorate(request, service);
                }

                serviceCache?.Store(Definition, service);
                if (service is IDisposable disposable) {
                    serviceTracker?.Track(disposable);
                }
            }

            return service;
        }

        public IEnumerable<IDecorator<TService>> GetDecorators(ServiceRequest request)
        {
            var req = request.CreateDependencyRequest(typeof(IEnumerable<IDecorator<TService>>));
            return (IEnumerable<IDecorator<TService>>)req.Container.GetService(req);
        }
    }
}
