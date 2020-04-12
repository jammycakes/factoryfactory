using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl
{
    public interface ILifecycleClause
    {
        IDefinitionClause Lifecycle(ILifecycle lifecycle);

        IDefinitionClause Lifecycle(ServiceLifetime lifetime);

        IDefinitionClause Singleton();

        IDefinitionClause Scoped();

        IDefinitionClause Transient();

        IDefinitionClause Untracked();
    }

    public interface ILifecycleClause<TService>
        where TService : class
    {
        IDefinitionClause<TService> Lifecycle(ILifecycle lifecycle);

        IDefinitionClause<TService> Lifecycle(ServiceLifetime lifetime);

        IDefinitionClause<TService> Singleton();

        IDefinitionClause<TService> Scoped();

        IDefinitionClause<TService> Transient();

        IDefinitionClause<TService> Untracked();
    }
}
