namespace FactoryFactory.Resolution
{
    /// <summary>
    ///  Implements the service caching component of lifecycle management.
    /// </summary>
    public class ServiceCacheResolver : IResolver
    {
        private readonly IResolver _innerResolver;
        private readonly ILifecycle _lifecycle;
        private readonly object _key;

        public ServiceCacheResolver(IServiceDefinition definition, ExpressionResolver innerResolver)
        {
            _innerResolver = innerResolver;
            _lifecycle = definition.Lifecycle;
            _key = innerResolver.Key;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority => _innerResolver.Priority;

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request)
        {
            var cache = _lifecycle.GetCache(request);
            if (cache != null) {
                var service = cache.Retrieve(_key);
                if (service == null) {
                    service = _innerResolver.GetService(request);
                    cache.Store(_key, service);
                }

                return service;
            }
            else {
                return _innerResolver.GetService(request);
            }
        }
    }
}
