using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nooshka
{
    public class Container : IServiceProvider, IDisposable
    {
        public Container(IServiceCollection services)
        {}

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
