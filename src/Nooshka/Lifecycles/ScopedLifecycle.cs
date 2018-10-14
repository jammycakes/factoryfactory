namespace Nooshka.Lifecycles
{
    public class ScopedLifecycle : Lifecycle
    {
        public override ILifecycleManager GetLifecycleManager(ServiceRequest request)
        {
            return request.Container.LifecycleManager;
        }

        public override Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container;
        }
    }
}
