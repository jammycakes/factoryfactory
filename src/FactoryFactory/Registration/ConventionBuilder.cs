using System;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory.Registration
{
    public class ConventionBuilder : IConventionClause
    {
        public ConventionBuilder(Module module, Action<IConventionPredicate> types)
        {
            throw new NotImplementedException();
        }

        public IOptionsClause As(Action<IConventionByName> byName)
        {
            throw new NotImplementedException();
        }

        public IOptionsClause From(Action<IConventionByScan> byScan)
        {
            throw new NotImplementedException();
        }
    }
}
