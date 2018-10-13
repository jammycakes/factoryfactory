using System;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Lifecycle;
using Nooshka.Registration;

namespace Nooshka
{
    public static class IModuleExtensions
    {
        /* ====== Add overloads ====== */

        public static IImplementation Add(this IModule module, Type serviceType)
        {
            var registration = new RegistrationBuilder(serviceType);
            module.Add(registration);
            return registration;
        }

        public static IImplementation<TService> Add<TService>(this IModule module)
        {
            var registration = new RegistrationBuilder<TService>();
            module.Add(registration);
            return registration;
        }

        public static void Add(this IModule module, IServiceCollection services)
        {
            foreach (var service in services) {
                module.Add(new ServiceRegistration(service));
            }
        }


        /* ====== Lifecycle overloads ====== */

        private static readonly ILifecycle _singletonLifecycle = new SingletonLifecycle();
        private static readonly ILifecycle _scopedLifecycle = new ScopedLifecycle();
        private static readonly ILifecycle _transientLifecycle = new TransientLifecycle();

        public static IOptions Singleton(this IOptions options) =>
            options.WithLifecycle(_singletonLifecycle);

        public static IOptions Scoped(this IOptions options) =>
            options.WithLifecycle(_scopedLifecycle);

        public static IOptions Transient(this IOptions options) =>
            options.WithLifecycle(_transientLifecycle);
    }
}
