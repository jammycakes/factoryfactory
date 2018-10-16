using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nooshka.Resolution
{
    public class ResolverCache : IDisposable
    {
        private readonly IList<IModule> _modules;

        private ReaderWriterLockSlim _resolverLock = new ReaderWriterLockSlim();
        private IDictionary<Type, List<Resolver>> _resolvers
            = new Dictionary<Type, List<Resolver>>();

        public ResolverCache(IEnumerable<IModule> modules)
        {
            _modules = modules.ToList();
        }

        public IEnumerable<Resolver> GetResolvers(Type type)
        {
            _resolverLock.EnterUpgradeableReadLock();
            try {
                List<Resolver> result;
                if (!_resolvers.TryGetValue(type, out result)) {
                    _resolverLock.EnterWriteLock();
                    try {
                        if (!_resolvers.TryGetValue(type, out result)) {
                            var builders =
                                from module in _modules
                                from registration in module.GetRegistrations(type)
                                select new ResolverBuilder(registration, this);
                            var builtResolvers =
                                from builder in builders
                                select builder.Build();
                            result = builtResolvers.ToList();
                            _resolvers.Add(type, result);
                        }
                    }
                    finally {
                        _resolverLock.ExitWriteLock();
                    }
                }

                return result;
            }
            finally {
                _resolverLock.ExitUpgradeableReadLock();
            }
        }

        public bool IsTypeRegistered(Type type)
            => _modules.Any(m => m.IsTypeRegistered(type));

        public void Dispose()
        {
            _resolverLock?.Dispose();
        }
    }
}
