namespace Nooshka.Lifecycles
{
    public class TransientLifecycle : Lifecycle
    {
        public override Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container;
        }

        public override bool IsTracked => false;
    }
}
