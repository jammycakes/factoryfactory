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
                group lazy by GetKey(lazy.Value.ServiceType)
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


        private Type GetKey(Type type)
        {
            if (!type.IsGenericType) return type;
            if (type.IsGenericTypeDefinition) return type;
            return type.GetGenericTypeDefinition();
        }


        private List<Lazy<ServiceDefinition>> GetServiceDefinitions(Type type, bool create)
        {
            type = GetKey(type);
            List<Lazy<ServiceDefinition>> result = null;
            if (!_definitions.TryGetValue(type, out result)) {
                if (create) {
                    result = new List<Lazy<ServiceDefinition>>();
                    _definitions.Add(type, result);
                }
            }

            return result;
        }

        private void Add(Type type, ServiceDefinition serviceDefinition)
        {
            Add(type, () => serviceDefinition);
        }

        public void Add(ServiceDefinition serviceDefinition)
        {
            Add(serviceDefinition.ServiceType, () => serviceDefinition);
        }

        public void Add(Type type, Func<ServiceDefinition> serviceDefinition)
        {
            type = GetKey(type);
            var list = GetServiceDefinitions(type, true);
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
            var key = GetKey(type);
            var definitions =
                GetServiceDefinitions(key, false)?.Select(x => x.Value)
                ?? Enumerable.Empty<ServiceDefinition>();

            return
                from definition in definitions
                where definition.ServiceType.IsGenericTypeDefinition
                    || definition.ServiceType == type
                select definition.GetGenericDefinition(type);
        }

        bool IModule.IsTypeRegistered(Type type) =>
            _definitions.ContainsKey(type) ||
            (type.IsGenericTypeDefinition &&
             _definitions.ContainsKey(type.GetGenericTypeDefinition()));
    }
}
