namespace Nooshka.Lifecycle
{
    public class TransientLifetime : ILifetime
    {
        public ILifetimeManager GetLifetimeManager(ServiceRequest request)
        {
            return null;
        }
    }
}
