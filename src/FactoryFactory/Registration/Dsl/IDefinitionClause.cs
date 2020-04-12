using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace FactoryFactory.Registration.Dsl
{
    public interface IDefinitionClause : ILifecycleClause, IPreconditionClause, IAsClause
    {
    }

    public interface IDefinitionClause<TService> :
        IDefinitionClause,
        ILifecycleClause<TService>,
        IPreconditionClause<TService>,
        IAsClause<TService>
        where TService : class
    {
    }
}
