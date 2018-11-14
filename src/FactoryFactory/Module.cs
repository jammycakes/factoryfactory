using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Registration;
using FactoryFactory.Registration.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class Module
    {
        private List<Lazy<IServiceDefinition>> _definitions = new List<Lazy<IServiceDefinition>>();

        public Module(params IServiceDefinition[] serviceDefinitions)
        {
            _definitions.AddRange(
                serviceDefinitions.Select(d => new Lazy<IServiceDefinition>(() => d))
            );
        }

        public Module(IServiceCollection services)
            : this(services.Select(s => new ServiceDefinition(s)).ToArray())
        { }

        protected Module()
        {
            _definitions = new List<Lazy<IServiceDefinition>>();
        }

        /* ====== Registration ====== */


        public void Add(IServiceDefinition serviceDefinition)
        {
            Add(() => serviceDefinition);
        }

        public IEnumerable<IServiceDefinition> GetServiceDefinitions()
        {
            return _definitions.Select(d => d.Value);
        }

        public void Add(Func<IServiceDefinition> serviceDefinition)
        {
            _definitions.Add(new Lazy<IServiceDefinition>(serviceDefinition));
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

        public IConventionClause Define(Action<IConventionPredicate> types)
        {
            return new ConventionBuilder(this, types);
        }

        public InterceptionBuilder<TService> Intercept<TService>() where TService : class
        {
            return new InterceptionBuilder<TService>(this);
        }
    }
}
