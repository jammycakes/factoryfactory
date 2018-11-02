using System;
using System.Collections.Generic;

namespace FactoryFactory.Impl
{
    public class ServiceTracker : IServiceTracker
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();
        private bool _disposing;

        public void Track(IDisposable service)
        {
            if (service != null) {
                _services.AddFirst(service);
            }
        }

        public void Dispose()
        {
            if (_disposing) return;
            try {
                _disposing = true;
                foreach (var service in _services) {
                    service.Dispose();
                }
            }
            finally {
                _disposing = false;
            }
        }
    }
}
