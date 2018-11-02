using System.Collections.Generic;

namespace FactoryFactory.Resolution
{
    public class DecoratorResolver<TService> : IResolver
    {
        private readonly IResolver _innerResolver;
        private readonly Configuration _configuration;

        public DecoratorResolver(IResolver innerResolver, Configuration configuration)
        {
            _innerResolver = innerResolver;
            _configuration = configuration;
            Priority = innerResolver.Priority;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority { get; }

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request)
        {
            TService service = (TService)_innerResolver.GetService(request);
            var decoratorRequest = request.CreateDependencyRequest
                    (typeof(IEnumerable<IDecorator<TService>>));
            var decorators = (IEnumerable<IDecorator<TService>>)
                request.Container.GetService(decoratorRequest);
            foreach (var decorator in decorators) {
                service = decorator.Decorate(request, service);
            }

            return service;
        }
    }
}
