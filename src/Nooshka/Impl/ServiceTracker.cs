using System;
using System.Collections.Generic;

namespace Nooshka.Impl
{
    public class ServiceTracker : IServiceTracker
    {
        private LinkedList<IDisposable> _services = new LinkedList<IDisposable>();

        public void Track(IDisposable service)
        {
            _services.AddFirst(service);
        }

        public void Dispose()
        {
            foreach (var service in _services) {
                service.Dispose();
            }
        }
    }
}
