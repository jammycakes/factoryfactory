using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;

namespace Nooshka
{
    public class Module : IModule
    {
        private IDictionary<Type, List<ServiceDefinition>> _registrations;

        public Module(params ServiceDefinition[] serviceDefinitions)
        {
            var registrationsByType =
                from registration in serviceDefinitions
                group registration by registration.ServiceType
                into byType
                select byType;

            _registrations = registrationsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select<ServiceDescriptor, ServiceDefinition>
                (s => new ServiceDefinition(s)).ToArray())
        {
        }

        protected Module()
        {
            _registrations = new Dictionary<Type, List<ServiceDefinition>>();
        }

        /* ====== Registration ====== */

        private List<ServiceDefinition> GetServiceRegistrations(Type type, bool create)
        {
            List<ServiceDefinition> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<ServiceDefinition>();
                    _registrations.Add(type, result);
                }
            }

            return result;
        }

        private void Add(Type registrationType, ServiceDefinition serviceDefinition)
        {
            var list = GetServiceRegistrations(registrationType, true);
            list.Add(serviceDefinition);
        }

        public void Add(ServiceDefinition serviceDefinition)
        {
            var list = GetServiceRegistrations(serviceDefinition.ServiceType, true);
            list.Add(serviceDefinition);
        }

        public void Add(IServiceCollection services)
        {
            foreach (var service in services) {
                Add(new ServiceDefinition(service));
            }
        }


        /* ====== Fluent registration ====== */

        public RegistrationBuilder Resolve(Type type)
        {
            var registration = new ServiceDefinition(type);
            Add(registration);
            return new RegistrationBuilder(registration);
        }

        public RegistrationBuilder<TService> Resolve<TService>()
        {
            var registration = new ServiceDefinition(typeof(TService));
            Add(registration);
            return new RegistrationBuilder<TService>(registration);
        }


        /* ====== Resolution ====== */

        IEnumerable<ServiceDefinition> IModule.GetRegistrations(Type type)
        {
            return (GetServiceRegistrations(type, false) ?? Enumerable.Empty<ServiceDefinition>())
                .AsEnumerable();
        }

        bool IModule.IsTypeRegistered(Type type) => _registrations.ContainsKey(type);
    }
}
