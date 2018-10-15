using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Nooshka.Registration
{
    public class Module : IModule
    {
        private IDictionary<Type, List<Registration>> _registrations;

        public Module(params Registration[] registrations)
        {
            var registrationsByType =
                from registration in registrations
                group registration by registration.ServiceType
                into byType
                select byType;

            _registrations = registrationsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select<ServiceDescriptor, Registration>
                (s => new Registration(s)).ToArray())
        {
        }

        protected Module()
        {
            _registrations = new Dictionary<Type, List<Registration>>();
        }

        /* ====== Registration ====== */

        private List<Registration> GetServiceRegistrations(Type type, bool create)
        {
            List<Registration> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<Registration>();
                    _registrations.Add(type, result);
                }
            }

            return result;
        }

        private void Add(Type registrationType, Registration registration)
        {
            var list = GetServiceRegistrations(registrationType, true);
            list.Add(registration);
        }

        public void Add(Registration registration)
        {
            var list = GetServiceRegistrations(registration.ServiceType, true);
            list.Add(registration);
        }

        public void Add(IServiceCollection services)
        {
            foreach (var service in services) {
                Add(new Registration(service));
            }
        }


        /* ====== Fluent registration ====== */

        public RegistrationBuilder Resolve(Type type)
        {
            var registration = new Registration(type);
            Add(registration);
            return new RegistrationBuilder(registration);
        }

        public RegistrationBuilder<TService> Resolve<TService>()
        {
            var registration = new Registration(typeof(TService));
            Add(registration);
            return new RegistrationBuilder<TService>(registration);
        }


        /* ====== Resolution ====== */

        IEnumerable<Registration> IModule.GetRegistrations(Type type)
        {
            return (GetServiceRegistrations(type, false) ?? Enumerable.Empty<Registration>())
                .AsEnumerable();
        }

        bool IModule.IsTypeRegistered(Type type) => _registrations.ContainsKey(type);
    }
}
