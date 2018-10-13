using System;
using System.Collections.Generic;
using System.Linq;

namespace Nooshka.Impl
{
    public class LifecycleManager : ILifecycleManager
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();

        public LifecycleManager(IServiceResolver serviceResolver)
        {
            ServiceResolver = serviceResolver;
        }

        public void Add(IDisposable managedService)
        {
            _services.AddFirst(managedService);
        }

        public IServiceResolver ServiceResolver { get; }

        public void Dispose()
        {
            foreach (var service in _services) {
                service.Dispose();
            }
        }
    }
}
