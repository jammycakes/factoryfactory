namespace FactoryFactory.Lifecycles
{
    public class TransientLifecycle : Lifecycle, IServiceCache
    {
        public override IServiceTracker GetTracker(ServiceRequest request)
            => request.Container.ServiceTracker;

        public override IServiceCache GetCache(ServiceRequest request)
            => this;

        public void Store(ServiceDefinition serviceDefinition, object service)
        {
        }

        public object Retrieve(ServiceDefinition serviceDefinition) => null;
    }
}
