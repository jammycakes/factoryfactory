namespace Nooshka.Lifecycles
{
    public class ScopedLifecycle : Lifecycle
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
        {
            return request.Container.ServiceTracker;
        }

        public override IServiceCache GetCache(ServiceRequest request)
        {
            return request.Container.ServiceCache;
        }
    }
}
