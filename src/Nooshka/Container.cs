using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;
using Nooshka.Util;

namespace Nooshka
{
    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    public class Container : IServiceProvider, IServiceScope, IServiceScopeFactory
    {
        private readonly Configuration _configuration;

        public IServiceCache ServiceCache { get; }

        public IServiceTracker ServiceTracker { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(Container parent)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            ServiceCache = new ServiceCache();
            ServiceTracker = new ServiceTracker();
            _configuration = Root._configuration;
        }

        public Container(Configuration configuration) : this(parent: null)
        {
            _configuration = configuration;
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            var request = new ServiceRequest(this, serviceType, null);
            var result = GetService(request);
            return result;
        }

        public object GetService(ServiceRequest request)
        {
            if (request.RequestedType.IsEnumerable()) {
                return GetAll(request);
            }
            else {
                return GetOne(request);
            }
        }

        private object GetAll(ServiceRequest request)
        {
            var resolvers = GetResolvers(request);
            var services = resolvers.Select(r => r.GetService(request)).ToArray();
            var result = Array.CreateInstance(request.RequestedType.GetServiceType(), services.Length);
            Array.Copy(services, result, services.Length);
            return result;
        }

        private object GetOne(ServiceRequest request)
        {
            var resolver = GetResolvers(request).LastOrDefault();
            if (resolver == null) return null;
            return resolver.GetService(request);
        }


        private IEnumerable<IServiceBuilder> GetResolvers(ServiceRequest serviceRequest)
        {
            return
                from resolver in _configuration.GetResolvers(serviceRequest.RequestedType.GetServiceType())
                where resolver.PreconditionMet(serviceRequest)
                select resolver;
        }


        /* ====== Release ====== */

        private bool _disposing = false;

        public void Dispose()
        {
            if (_disposing) return;
            _disposing = true;
            try {
                ServiceTracker.Dispose();
            }
            finally {
                _disposing = false;
            }
        }

        /* ====== Hierarchy ====== */

        public Container Parent { get; }

        public Container Root { get; }

        public Container CreateChild()
        {
            var child = new Container(this);
            return child;
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return CreateChild();
        }
    }
}
