using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Registration.Fluent;

namespace Nooshka.Registration
{
    public class Module : IModule
    {
        private IDictionary<Type, List<IRegistration>> _registrations;

        public Module(params IRegistration[] registrations)
        {
            var registrationsByType =
                from registration in registrations
                group registration by registration.ServiceType
                into byType
                select byType;

            _registrations = registrationsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select<ServiceDescriptor, IRegistration>
                (s => new ServiceRegistration(s)).ToArray())
        {
        }

        protected Module()
        {
            _registrations = new Dictionary<Type, List<IRegistration>>();
        }

        private List<IRegistration> GetServiceRegistrations(Type type, bool create)
        {
            List<IRegistration> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<IRegistration>();
                    _registrations.Add(type, result);

                }
            }

            return result;
        }

        protected void Add(IRegistration registration)
        {
            var list = GetServiceRegistrations(registration.ServiceType, true);
            list.Add(registration);
        }

        protected IImplementation Add(Type serviceType)
        {
            var registration = new RegistrationBuilder(serviceType);
            Add(registration);
            return registration;
        }

        protected IImplementation<TService> Add<TService>()
        {
            var registration = new RegistrationBuilder<TService>();
            Add(registration);
            return registration;
        }

        IEnumerable<IRegistration> IModule.GetServiceRegistrations(Type type)
        {
            return (GetServiceRegistrations(type, false) ?? Enumerable.Empty<IRegistration>())
                .AsEnumerable();
        }
    }
}
