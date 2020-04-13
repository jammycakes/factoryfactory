using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Registration;
using FactoryFactory.Registration.Impl;
using FactoryFactory.Registration.Impl.ServiceDefinitions;
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

        public IConventionDefinitionClause Define(Action<IConventionPredicates> types)
        {
            return new ConventionDefinitionBuilder(this, types);
        }

        public IInterceptionDefinitionClause<TService> Intercept<TService>() where TService : class
        {
            return new InterceptionDefinitionBuilder<TService>(this);
        }
    }
}
