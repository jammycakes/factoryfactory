using System;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl
{
    public interface IOptionsClause<TReturn>
    {
        TReturn Lifecycle(ILifecycle lifecycle);

        TReturn Lifecycle(ServiceLifetime lifetime);

        TReturn Singleton();

        TReturn Scoped();

        TReturn Transient();

        TReturn Untracked();

        TReturn Precondition(Func<ServiceRequest, bool> precondition);
    }

    public interface IOptionsClause<TService, TReturn>
        where TService : class
    {
        TReturn Lifecycle(ILifecycle lifecycle);

        TReturn Lifecycle(ServiceLifetime lifetime);

        TReturn Singleton();

        TReturn Scoped();

        TReturn Transient();

        TReturn Untracked();

        TReturn Precondition(Func<ServiceRequest, bool> precondition);
    }
}
