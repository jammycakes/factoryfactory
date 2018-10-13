using System;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Registration;

namespace Nooshka
{
    public static class IModuleExtensions
    {
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
    }
}
