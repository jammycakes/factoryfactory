using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Impl;
using FactoryFactory.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class Module : IModule
    {
        private IDictionary<Type, List<Lazy<ServiceDefinition>>> _definitions;

        public Module(params ServiceDefinition[] serviceDefinitions)
        {
            var definitionsByType =
                from definition in serviceDefinitions
                let lazy = new Lazy<ServiceDefinition>(() => definition)
                group lazy by lazy.Value.ServiceType
                into byType
                select byType;

            _definitions = definitionsByType.ToDictionary(k => k.Key, v => v.ToList());
        }

        public Module(IServiceCollection services)
            : this(services.Select(s => new ServiceDefinition(s)).ToArray())
        {
        }

        protected Module()
        {
            _definitions = new Dictionary<Type, List<Lazy<ServiceDefinition>>>();
        }

        /* ====== Registration ====== */

        private List<Lazy<ServiceDefinition>> GetServiceDefinitions(Type type, bool create)
        {
            List<Lazy<ServiceDefinition>> result = null;
            if (!_definitions.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<Lazy<ServiceDefinition>>();
                    _definitions.Add(type, result);
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
            var list = GetServiceDefinitions(registrationType, true);
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

        IEnumerable<ServiceDefinition> IModule.GetDefinitions(Type type)
        {
            var definitions =
                GetServiceDefinitions(type, false) ?? Enumerable.Empty<Lazy<ServiceDefinition>>();
            if (type.IsGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericDefinitions = (GetServiceDefinitions(genericType, false)) ??
                                         Enumerable.Empty<Lazy<ServiceDefinition>>();
                definitions = definitions.Concat(genericDefinitions);
            }

            return definitions.Select(x => x.Value);
        }

        bool IModule.IsTypeRegistered(Type type) =>
            _definitions.ContainsKey(type) ||
            (type.IsGenericTypeDefinition &&
             _definitions.ContainsKey(type.GetGenericTypeDefinition()));
    }
}
