using System;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory.Registration
{
    public class ConventionBuilder : IConventionClause, IOptionsClause
    {
        private IConventionPredicates _predicates = new ConventionPredicates();
        private ILifecycle _lifecycle = null;
        private Func<ServiceRequest, bool> _precondition = null;

        public ConventionBuilder(Module module, Action<IConventionPredicates> buildPredicates)
        {
            buildPredicates(_predicates);
            module.Add(BuildConvention);
        }

        IOptionsClause IConventionClause.As(Action<IConventionByName> buildNameConvention)
        {
            throw new NotImplementedException();
        }

        IOptionsClause IConventionClause.From(Action<IConventionByScan> buildScanConvention)
        {
            throw new NotImplementedException();
        }

        IOptionsClause IOptionsClause.Lifecycle(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
            return this;
        }

        IOptionsClause IOptionsClause.Precondition(Func<ServiceRequest, bool> precondition)
        {
            _precondition = precondition;
            return this;
        }

        private IServiceDefinition BuildConvention()
        {
            throw new NotImplementedException();
        }
    }
}
