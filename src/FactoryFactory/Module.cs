using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
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

        public RegistrationBuilder Define(Type type)
        {
            var registration = new ServiceDefinition(type);
            Add(registration);
            return new RegistrationBuilder(registration);
        }

        public RegistrationBuilder<TService> Define<TService>()
        {
            var registration = new ServiceDefinition(typeof(TService));
            Add(registration);
            return new RegistrationBuilder<TService>(registration);
        }


        /* ====== Resolution ====== */

        IEnumerable<ServiceDefinition> IModule.GetRegistrations(Type type)
        {
            var definitions =
                GetServiceRegistrations(type, false) ?? Enumerable.Empty<ServiceDefinition>();
            if (type.IsGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericDefinitions = (GetServiceRegistrations(genericType, false)) ??
                                         Enumerable.Empty<ServiceDefinition>();
                definitions = definitions.Concat(genericDefinitions);
            }

            return definitions;
        }

        bool IModule.IsTypeRegistered(Type type) =>
            _registrations.ContainsKey(type) ||
            (type.IsGenericTypeDefinition &&
             _registrations.ContainsKey(type.GetGenericTypeDefinition()));
    }
}
