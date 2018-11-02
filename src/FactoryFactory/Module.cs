using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class Module
    {
        private List<Lazy<ServiceDefinition>> _definitions = new List<Lazy<ServiceDefinition>>();

        public Module(params ServiceDefinition[] serviceDefinitions)
        {
            _definitions.AddRange(
                serviceDefinitions.Select(
                    d => new Lazy<ServiceDefinition>(
                        () => d)));
        }

        public Module(IServiceCollection services)
            : this(services.Select(s => new ServiceDefinition(s)).ToArray())
        { }

        protected Module()
        {
            _definitions = new List<Lazy<ServiceDefinition>>();
        }

        /* ====== Registration ====== */


        public void Add(ServiceDefinition serviceDefinition)
        {
            Add(() => serviceDefinition);
        }

        public IEnumerable<ServiceDefinition> GetServiceDefinitions()
        {
            return _definitions.Select(d => d.Value);
        }

        public void Add(Func<ServiceDefinition> serviceDefinition)
        {
            _definitions.Add(new Lazy<ServiceDefinition>(serviceDefinition));
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

        public DefinitionBuilder<TService> Define<TService>() where TService: class
        {
            return new DefinitionBuilder<TService>(this);
        }

        public DecorationBuilder<TService> Decorate<TService>() where TService : class
        {
            return new DecorationBuilder<TService>(this);
        }
    }
}
