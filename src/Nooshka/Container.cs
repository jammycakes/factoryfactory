using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;

namespace Nooshka
{
    /// <summary>
    ///  The core class which is responsible for resolution and management of
    ///  requested services.
    /// </summary>
    public class Container : IServiceProvider, IServiceScope
    {
        private IServiceResolver _resolver;

        /// <summary>
        ///  Gets the default <see cref="ILifecycleManager"/> instance which
        ///  tracks service lifetimes that correspond to the duration of this
        ///  service.
        /// </summary>
        public ILifecycleManager LifecycleManager { get; } = new LifecycleManager();

        public ICollection<IModule> Modules { get; }

        public IServiceProvider ServiceProvider => this;

        private Container(Container parent)
        {
            Parent = parent;
            Root = parent?.Root ?? this;
            _resolver = new ServiceResolver(this);
        }

        public Container(params IModule[] modules) : this(parent: null)
        {
            Modules = new ReadOnlyCollection<IModule>(modules.ToList());
        }


        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            var request = new ServiceRequest(this, serviceType, null);
            return _resolver.GetService(request);
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
            return new Container(this);
        }
    }
}
