namespace Nooshka.Lifecycles
{
    public class TransientLifecycle : Lifecycle
    {
        public override ILifecycleManager GetLifecycleManager(ServiceRequest request)
        {
            return null;
        }

        public override Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container;
        }
    }
}
