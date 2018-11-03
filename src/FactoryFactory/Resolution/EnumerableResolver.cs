using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryFactory.Resolution
{
    /// <summary>
    ///  Provides resolution service for IEnumerable<TService>.
    /// </summary>
    public class EnumerableResolver<TService> : IResolver
    {
        private readonly IList<IResolver> _resolvers;

        public EnumerableResolver(IEnumerable<IResolver> resolvers)
        {
            _resolvers = resolvers.Where(r => r.CanResolve).ToList();
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public int Priority => 0;

        public Type Type => typeof(TService);

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request)
        {
            return _resolvers
                .Where(r => r.IsConditionMet(request))
                .Select(r => r.GetService(request))
                .Cast<TService>()
                .ToList();
        }

        public override string ToString() => $"EnumerableResolver for {Type}";
    }
}
