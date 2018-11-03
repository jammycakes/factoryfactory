using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryFactory.Resolution
{
    /// <summary>
    ///  Provides resolution services for a single instance of TRequest,
    ///  where one or more of the resolvers are conditional resolvers.
    /// </summary>
    public class SingleResolver : IResolver
    {
        private readonly IEnumerable<IResolver> _resolvers;

        public SingleResolver(IEnumerable<IResolver> resolvers, Type type)
        {
            _resolvers = resolvers.OrderBy(r => r.Priority).Reverse().ToList();
            if (!_resolvers.Any()) {
                ActualResolver = new NonResolver(type);
            }
            else if (!_resolvers.First().Conditional) {
                ActualResolver = _resolvers.First();
            }
            else {
                ActualResolver = this;
            }

            Type = type;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority => 0;

        public Type Type { get; }

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request) =>
            _resolvers.FirstOrDefault(r => r.IsConditionMet(request))
                ?.GetService(request);

        public IResolver ActualResolver { get; }

        public override string ToString() => $"SingleResolver for {Type}";
    }
}
