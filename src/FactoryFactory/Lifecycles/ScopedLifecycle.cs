namespace FactoryFactory.Lifecycles
{
    public class ScopedLifecycle : ILifecycle
    {
        public bool Cached => true;

        public bool Tracked => true;

        public IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public IServiceCache GetCache(ServiceRequest request)
            => request.Container.ServiceCache;
    }
}
