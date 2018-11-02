using System;

namespace FactoryFactory.Lifecycles
{
    public class UntrackedLifecycle : Lifecycle, IServiceTracker, IServiceCache
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => this;

        public override IServiceCache GetCache(ServiceRequest request)
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
