using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Registration;

namespace Nooshka
{
    public class Module : IModule
    {
        private IDictionary<Type, List<Registration.Registration>> _registrations;

        public Module(params Registration.Registration[] registrations)
        {
            var registrationsByType =
                from registration in registrations
                group registration by registration.ServiceType
                into byType
                select byType;

            _registrations = registrationsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select<ServiceDescriptor, Registration.Registration>
                (s => new Registration.Registration(s)).ToArray())
        {
        }

        protected Module()
        {
            _registrations = new Dictionary<Type, List<Registration.Registration>>();
        }

        /* ====== Registration ====== */

        private List<Registration.Registration> GetServiceRegistrations(Type type, bool create)
        {
            List<Registration.Registration> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<Registration.Registration>();
                    _registrations.Add(type, result);
                }
            }

            return result;
        }

        private void Add(Type registrationType, Registration.Registration registration)
        {
            var list = GetServiceRegistrations(registrationType, true);
            list.Add(registration);
        }

        public void Add(Registration.Registration registration)
        {
            var list = GetServiceRegistrations(registration.ServiceType, true);
            list.Add(registration);
        }

        public void Add(IServiceCollection services)
        {
            foreach (var service in services) {
                Add(new Registration.Registration(service));
            }
        }


        /* ====== Fluent registration ====== */

        public RegistrationBuilder Resolve(Type type)
        {
            var registration = new Registration.Registration(type);
            Add(registration);
            return new RegistrationBuilder(registration);
        }

        public RegistrationBuilder<TService> Resolve<TService>()
        {
            var registration = new Registration.Registration(typeof(TService));
            Add(registration);
            return new RegistrationBuilder<TService>(registration);
        }


        /* ====== Resolution ====== */

        IEnumerable<Registration.Registration> IModule.GetRegistrations(Type type)
        {
            return (GetServiceRegistrations(type, false) ?? Enumerable.Empty<Registration.Registration>())
                .AsEnumerable();
        }

        bool IModule.IsTypeRegistered(Type type) => _registrations.ContainsKey(type);
    }
}
