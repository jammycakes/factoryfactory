using System;
using System.Linq.Expressions;

namespace FactoryFactory.Registration
{
    public interface IDefinitionClause : IOptionsClause<IDefinitionClause>
    {
        Registry As(Type implementationType);

        Registry As(object instance);

        Registry As(Expression<Func<ServiceRequest, object>> factory);
    }

    public interface IDefinitionClause<TService> :
        IOptionsClause<TService, IDefinitionClause<TService>>
        where TService : class
    {
        Registry As<TImplementation>() where TImplementation : TService;

        Registry As(TService instance);

        Registry As(Expression<Func<ServiceRequest, TService>> factory);
    }
}
