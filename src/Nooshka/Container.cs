using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Modules;

namespace Nooshka
{
    public class Container : IServiceProvider, IDisposable
    {
        private IList<IModule> _modules = new List<IModule>();

        public Container()
        {}

        public Container(IServiceCollection services)
        {
            AddModule(new ServiceCollectionModule(services));
        }

        /* ====== Register ====== */

        public void AddModule(IModule module)
        {
            _modules.Add(module);
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
