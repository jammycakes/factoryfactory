namespace FactoryFactory.Lifecycles
{
    public class TransientLifecycle : Lifecycle, IServiceCache
    {
        public override bool Cached => false;

        public override bool Tracked => true;

        public override IServiceCache GetCache(ServiceRequest request)
            => this;

        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public void Store(object key, object service)
        {
        }

        public object Retrieve(object key) => null;
    }
}
