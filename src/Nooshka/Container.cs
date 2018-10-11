using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Registries;

namespace Nooshka
{
    public class Container : IServiceProvider, IDisposable
    {
        private IList<IRegistry> _registries = new List<IRegistry>();

        public Container()
        {}

        public Container(IServiceCollection services)
        {
            Register(new ServiceCollectionRegistry(services));
        }

        /* ====== Register ====== */

        public void Register(IRegistry registry)
        {
            _registries.Add(registry);
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
