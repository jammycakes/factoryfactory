using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Modules;

namespace Nooshka
{
    public class Container : IServiceProvider, IDisposable
    {
        private IList<IModule> _modules = new List<IModule>();

        public Container(params IModule[] modules)
        {
            _modules = new List<IModule>(modules);
        }

        /* ====== Resolve ====== */

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        /* ====== Release ====== */

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
