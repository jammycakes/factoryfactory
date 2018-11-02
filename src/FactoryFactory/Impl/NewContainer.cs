using System;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Impl
{

    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    internal class NewContainer : IContainer
    {
        private readonly Configuration _configuration;

        public IServiceCache ServiceCache { get; }

        public IServiceTracker ServiceTracker { get; }

        public IServiceProvider ServiceProvider => this;

        private NewContainer(IContainer parent, Configuration configuration)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            ServiceCache = new ServiceCache();
            ServiceTracker = new ServiceTracker();
            _configuration = configuration;
        }

        public NewContainer(Configuration configuration) : this(null, configuration)
        {
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            var request = new ServiceRequest(this, serviceType, null);
            return GetService(request);
        }

        public object GetService(ServiceRequest request)
        {
            var resolver = _configuration.GetResolver(request.RequestedType);
            return resolver?.GetService(request);
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

        public IContainer Parent { get; }

        public IContainer Root { get; }

        public IContainer CreateChild()
        {
            var child = new NewContainer(this, _configuration);
            return child;
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return CreateChild();
        }
    }
}
