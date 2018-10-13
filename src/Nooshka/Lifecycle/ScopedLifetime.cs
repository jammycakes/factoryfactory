namespace Nooshka.Lifecycle
{
    public class ScopedLifetime : ILifetime
    {
        public ILifetimeManager GetLifetimeManager(ServiceRequest request)
        {
            return request.Container.LifetimeManager;
        }
    }
}
