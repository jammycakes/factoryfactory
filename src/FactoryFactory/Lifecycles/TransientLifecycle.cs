namespace FactoryFactory.Lifecycles
{
    public class TransientLifecycle : ILifecycle, IServiceCache
    {
        public bool Cached => false;

        public bool Tracked => true;

        public IServiceCache GetCache(ServiceRequest request)
            => this;

        public IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public void Store(object key, object service)
        {
        }

        public object Retrieve(object key) => null;
    }
}
