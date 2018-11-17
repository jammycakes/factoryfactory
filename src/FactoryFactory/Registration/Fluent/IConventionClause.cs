using System;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionClause
    {
        IOptionsClause As(Action<IConventionByName> byName);

        IOptionsClause Scanning(Action<IConventionByScan> byScan);
    }
}
