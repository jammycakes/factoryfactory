using System;

namespace FactoryFactory.Resolution
{
    /// <summary>
    ///  Tracks lifecycle of IDisposable implementations.
    /// </summary>
    public class ServiceTrackerResolver : IResolver
    {
        private readonly IResolver _innerResolver;
        private readonly ILifecycle _lifecycle;

        public ServiceTrackerResolver(IServiceDefinition definition, IResolver innerResolver)
        {
            _innerResolver = innerResolver;
            _lifecycle = definition.Lifecycle;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority => _innerResolver.Priority;

        public Type Type => _innerResolver.Type;

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request)
        {
            var service = _innerResolver.GetService(request);
            _lifecycle?.GetTracker(request)?.Track(service as IDisposable);
            return service;
        }

        public override string ToString() => $"ServiceTrackerResolver for {Type}";
    }
}
