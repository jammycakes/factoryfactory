using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;
using Nooshka.Resolution;

namespace Nooshka
{
    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    public class Container : IServiceProvider, IServiceScope
    {
        private ResolverCache _resolverCache;

        public ILifecycleManager LifecycleManager { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(Container parent)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            LifecycleManager = new LifecycleManager(this);
        }

        public Container(params IModule[] modules) : this(parent: null)
        {
            _resolverCache = new ResolverCache(modules);
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            return GetService(new ServiceRequest(this, serviceType, null));
        }

        public object GetService(ServiceRequest serviceRequest)
        {
            var resolvers = _resolverCache.GetResolvers(serviceRequest.RequestedType);
            var resolver = resolvers.LastOrDefault();
            if (resolver == null) return null;
            return resolver.GetService(serviceRequest);
        }

        /* ====== Release ====== */

        public void Dispose()
        {
            LifecycleManager.Dispose();
        }

        /* ====== Hierarchy ====== */

        public Container Parent { get; }

        public Container Root { get; }

        public Container CreateChild()
        {
            var child = new Container(this);
            LifecycleManager.Add(child);
            return child;
        }
    }
}
