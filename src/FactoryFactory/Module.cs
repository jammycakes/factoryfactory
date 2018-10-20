using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class Module : IModule
    {
        private IDictionary<Type, List<Lazy<ServiceDefinition>>> _registrations;

        public Module(params ServiceDefinition[] serviceDefinitions)
        {
            var registrationsByType =
                from registration in serviceDefinitions
                let lazy = new Lazy<ServiceDefinition>(() => registration)
                group lazy by lazy.Value.ServiceType
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
            _registrations = new Dictionary<Type, List<Lazy<ServiceDefinition>>>();
        }

        /* ====== Registration ====== */

        private List<Lazy<ServiceDefinition>> GetServiceRegistrations(Type type, bool create)
        {
            List<Lazy<ServiceDefinition>> result = null;
            if (!_registrations.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<Lazy<ServiceDefinition>>();
                    _registrations.Add(type, result);
                }
            }

            return result;
        }

        private void Add(Type registrationType, ServiceDefinition serviceDefinition)
        {
            Add(registrationType, () => serviceDefinition);
        }

        public void Add(ServiceDefinition serviceDefinition)
        {
            Add(serviceDefinition.ServiceType, () => serviceDefinition);
        }

        public void Add(Type registrationType, Func<ServiceDefinition> serviceDefinition)
        {
            var list = GetServiceRegistrations(registrationType, true);
            list.Add(new Lazy<ServiceDefinition>(serviceDefinition));
        }

        public void Add(IServiceCollection services)
        {
            foreach (var service in services) {
                Add(new ServiceDefinition(service));
            }
        }


        /* ====== Fluent registration ====== */

        public DefinitionBuilder Define(Type type)
        {
            return new DefinitionBuilder(this, type);
        }

        public DefinitionBuilder<TService> Define<TService>()
        {
            return new DefinitionBuilder<TService>(this);
        }


        /* ====== Resolution ====== */

        IEnumerable<ServiceDefinition> IModule.GetRegistrations(Type type)
        {
            var definitions =
                GetServiceRegistrations(type, false) ?? Enumerable.Empty<Lazy<ServiceDefinition>>();
            if (type.IsGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericDefinitions = (GetServiceRegistrations(genericType, false)) ??
                                         Enumerable.Empty<Lazy<ServiceDefinition>>();
                definitions = definitions.Concat(genericDefinitions);
            }

            return definitions.Select(x => x.Value);
        }

        bool IModule.IsTypeRegistered(Type type) =>
            _registrations.ContainsKey(type) ||
            (type.IsGenericTypeDefinition &&
             _registrations.ContainsKey(type.GetGenericTypeDefinition()));
    }
}
