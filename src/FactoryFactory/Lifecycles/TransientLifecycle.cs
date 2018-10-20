namespace FactoryFactory.Lifecycles
{
    public class TransientLifecycle : Lifecycle
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public override IServiceCache GetCache(ServiceRequest request)
            => request.ServiceCache;
    }
}
