namespace Nooshka.Lifecycles
{
    public class SingletonLifecycle : Lifecycle
    {
        public override Container GetServicingContainer(ServiceRequest request)
        {
            return request.Container.Root;
        }

        public override bool IsTracked => true;
    }
}
