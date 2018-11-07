using System;
using System.Collections.Generic;

namespace FactoryFactory.Resolution
{
    public class InterceptorResolver<TService> : IResolver
    {
        private readonly IResolver _innerResolver;
        private readonly Configuration _configuration;

        public InterceptorResolver(IResolver innerResolver, Configuration configuration)
        {
            _innerResolver = innerResolver;
            _configuration = configuration;
            Priority = innerResolver.Priority;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority { get; }

        public Type Type => _innerResolver.Type;

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request)
        {
            Func<TService> service = () => (TService)_innerResolver.GetService(request);
            var interceptorRequest = request.CreateDependencyRequest
                    (typeof(IEnumerable<IInterceptor<TService>>));
            var interceptors = (IEnumerable<IInterceptor<TService>>)
                request.Container.GetService(interceptorRequest);
            foreach (var interceptor in interceptors) {
                var innerService = service;
                service = () => interceptor.Intercept(request, innerService);
            }

            return service();
        }

        public override string ToString() => $"DecoratorResolver for {Type}";
    }
}
