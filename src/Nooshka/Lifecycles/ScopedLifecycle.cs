namespace Nooshka.Lifecycles
{
    public class ScopedLifecycle : Lifecycle
    {
        public override Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container;
        }

        public override bool IsTracked => true;
    }
}
