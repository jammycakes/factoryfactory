using System;
using System.Collections.Generic;
using System.Linq;

namespace Nooshka.Impl
{
    public class LifetimeManager : ILifetimeManager
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();

        public void Add(IDisposable managedService)
        {
            _services.AddFirst(managedService);
        }

        public void Dispose()
        {
            foreach (var service in _services) {
                service.Dispose();
            }
        }
    }
}
