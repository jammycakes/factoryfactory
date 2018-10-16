using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;

namespace Nooshka
{
    public class Module : IModule
    {
        private IDictionary<Type, List<ServiceRegistration>> _registrations;

        public Module(params ServiceRegistration[] serviceRegistrations)
        {
            var registrationsByType =
                from registration in serviceRegistrations
                group registration by registration.ServiceType
                into byType
                select byType;

            _registrations = registrationsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select<ServiceDescriptor, ServiceRegistration>
                (s => new ServiceRegistration(s)).ToArray())
        {
        }

        protected Module()
        {
            _registrations = new Dictionary<Type, List<ServiceRegistration>>();
        }

        /* ====== Registration ====== */

        private List<ServiceRegistration> GetServiceRegistrations(Type type, bool create)
        {
            List<ServiceRegistration> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<ServiceRegistration>();
                    _registrations.Add(type, result);
                }
            }

            return result;
        }

        private void Add(Type registrationType, ServiceRegistration serviceRegistration)
        {
            var list = GetServiceRegistrations(registrationType, true);
            list.Add(serviceRegistration);
        }

        public void Add(ServiceRegistration serviceRegistration)
        {
            var list = GetServiceRegistrations(serviceRegistration.ServiceType, true);
            list.Add(serviceRegistration);
        }

        public void Add(IServiceCollection services)
        {
            foreach (var service in services) {
                Add(new ServiceRegistration(service));
            }
        }


        /* ====== Fluent registration ====== */

        public RegistrationBuilder Resolve(Type type)
        {
            var registration = new ServiceRegistration(type);
            Add(registration);
            return new RegistrationBuilder(registration);
        }

        public RegistrationBuilder<TService> Resolve<TService>()
        {
            var registration = new ServiceRegistration(typeof(TService));
            Add(registration);
            return new RegistrationBuilder<TService>(registration);
        }


        /* ====== Resolution ====== */

        IEnumerable<ServiceRegistration> IModule.GetRegistrations(Type type)
        {
            return (GetServiceRegistrations(type, false) ?? Enumerable.Empty<ServiceRegistration>())
                .AsEnumerable();
        }

        bool IModule.IsTypeRegistered(Type type) => _registrations.ContainsKey(type);
    }
}
