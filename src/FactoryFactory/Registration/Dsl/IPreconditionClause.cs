using System;
using System.Collections.Generic;
using System.Text;

namespace FactoryFactory.Registration.Dsl
{
    public interface IPreconditionClause
    {
        IDefinitionClause Precondition(Func<ServiceRequest, bool> precondition);
    }

    public interface IPreconditionClause<TService>
        where TService : class
    {
        IDefinitionClause<TService> Precondition(Func<ServiceRequest, bool> precondition);
    }
}
