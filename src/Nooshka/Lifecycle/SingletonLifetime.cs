namespace Nooshka.Lifecycle
{
    public class SingletonLifetime : ILifetime
    {
        public ILifetimeManager GetLifetimeManager(ServiceRequest request)
        {
            return request.Container.Root.LifetimeManager;
        }
    }
}
