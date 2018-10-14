using System;
using System.Collections.Generic;

namespace Nooshka.Impl
{
    public class LifecycleManager : ILifecycleManager
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();

        public LifecycleManager(Container container)
        {
            Container = container;
        }

        public void Add(IDisposable managedService)
        {
            _services.AddFirst(managedService);
        }

        public Container Container { get; }

        public void Dispose()
        {
            foreach (var service in _services) {
                service.Dispose();
            }
        }
    }
}
