using Microsoft.Extensions.DependencyInjection;
using Nooshka.Lifecycles;

namespace Nooshka
{
    public abstract class Lifecycle
    {
        public abstract ILifecycleManager GetLifecycleManager(ServiceRequest request);

        public abstract Container GetServicingContainer(ServiceRequest request);

        public static readonly Lifecycle Scoped = new ScopedLifecycle();
        public static readonly Lifecycle Singleton = new SingletonLifecycle();
        public static readonly Lifecycle Transient = new TransientLifecycle();
        public static Lifecycle Default => Transient;

        public static Lifecycle Get(ServiceLifetime lifetime)
        {
            switch (lifetime) {
                case ServiceLifetime.Scoped: return Scoped;
                case ServiceLifetime.Singleton: return Singleton;
                case ServiceLifetime.Transient: return Transient;
                default: return null;
            }
        }
    }
}
