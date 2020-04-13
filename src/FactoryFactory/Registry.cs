using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Registration.Dsl;
using FactoryFactory.Registration.ServiceDefinitions;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class Registry : List<ServiceDescriptor>, IServiceCollection, IRegistry
    {
        public Registry()
        { }

        public Registry(IServiceCollection other)
        {
            this.AddRange(other);
        }

        public IEnumerable<IServiceDefinition> GetServiceDefinitions() =>
            this.Select(desc => desc as IServiceDefinition ?? new ServiceDefinition(desc));

        /* ====== Fluent DSL ====== */

        public IDefinitionClause Define(Type serviceType)
        {
            return new DefinitionBuilder(this, serviceType);
        }

        public IDefinitionClause<TService> Define<TService>() where TService : class
        {
            return new DefinitionBuilder<TService>(this);
        }

        // TODO

        //public IConventionDefinitionClause Define(Action<IConventionPredicates> types)
        //{
        //}

        public IInterceptionDefinitionClause<TService> Intercept<TService>() where TService : class
        {
            return new InterceptionDefinitionBuilder<TService>(this);
        }
    }
}
