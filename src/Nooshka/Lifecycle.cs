using Microsoft.Extensions.DependencyInjection;
using Nooshka.Lifecycles;

namespace Nooshka
{
    public abstract class Lifecycle
    {
        public abstract IServiceTracker GetTracker(ServiceRequest request);

        public abstract IServiceCache GetCache(ServiceRequest request);

        public static readonly Lifecycle Null = new NullLifecycle();
        public static readonly Lifecycle Scoped = new ScopedLifecycle();
        public static readonly Lifecycle Singleton = new SingletonLifecycle();
        public static readonly Lifecycle Transient = new TransientLifecycle();
        public static readonly Lifecycle Untracked = new UntrackedLifecycle();

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
