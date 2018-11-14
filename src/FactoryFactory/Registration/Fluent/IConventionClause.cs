using System;

namespace FactoryFactory.Registration.Fluent
{
    public interface IConventionClause
    {
        IOptionsClause As(Action<IConventionByName> byName);

        IOptionsClause From(Action<IConventionByScan> byScan);
    }
}
