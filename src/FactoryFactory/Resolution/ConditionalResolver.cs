using System;

namespace FactoryFactory.Resolution
{
    public class ConditionalResolver : IResolver
    {
        private readonly IResolver _innerResolver;
        private Func<ServiceRequest, bool> _precondition;

        public ConditionalResolver(IServiceDefinition definition, IResolver innerResolver)
        {
            _innerResolver = innerResolver;
            _innerResolver = innerResolver;
            _precondition = definition.Precondition;
            Priority = _innerResolver.Priority;
        }

        public bool CanResolve => true;

        public bool Conditional => true;

        public int Priority { get; }

        public bool IsConditionMet(ServiceRequest request) => _precondition(request);

        public object GetService(ServiceRequest request) => _innerResolver.GetService(request);
    }
}
