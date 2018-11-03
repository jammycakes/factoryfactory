using System;

namespace FactoryFactory.Lifecycles
{
    public class UntrackedLifecycle : ILifecycle, IServiceTracker, IServiceCache
    {
        public bool Cached => false;

        public bool Tracked => false;

        public IServiceTracker GetTracker(ServiceRequest request)
            => this;

        public IServiceCache GetCache(ServiceRequest request)
            => this;

        public void Dispose()
        {
        }

        public void Track(IDisposable service)
        {
        }

        public void Store(object key, object service)
        {
        }

        public object Retrieve(object key)
            => null;
    }
}
