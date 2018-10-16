using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;

namespace Nooshka
{
    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    public class Container : IServiceProvider, IServiceScope, IServiceScopeFactory
    {
        private ResolverCache _resolverCache;

        public ILifecycleManager LifecycleManager { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(Container parent)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            LifecycleManager = new LifecycleManager();
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

        private IEnumerable<Resolver> GetResolvers(ServiceRequest serviceRequest)
        {
            return
                from resolver in _resolverCache.GetResolvers(serviceRequest.RequestedType)
                where resolver.PreconditionMet(serviceRequest)
                select resolver;
        }

        public object GetService(ServiceRequest request)
        {
            var resolver = GetResolvers(request).LastOrDefault();
            if (resolver == null) return null;
            return resolver.GetService(request);
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
            LifecycleManager.Track(child);
            return child;
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return CreateChild();
        }
    }
}
