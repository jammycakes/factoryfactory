using System;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory.Registration
{
    public class ConventionBuilder : IConventionClause, IOptionsClause
    {
        private ConventionPredicates _predicates = new ConventionPredicates();
        private ILifecycle _lifecycle = null;
        private Func<ServiceRequest, bool> _precondition = null;
        private ITypeFinderBuilder _typeFinderBuilder = null;

        public ConventionBuilder(Module module, Action<IConventionPredicates> buildPredicates)
        {
            buildPredicates(_predicates);
            module.Add(BuildConvention);
        }

        IOptionsClause IConventionClause.As(Action<IConventionByName> buildNameConvention)
        {
            var byName = new ConventionByNameBuilder();
            buildNameConvention(byName);
            _typeFinderBuilder = byName;
            return this;
        }

        IOptionsClause IConventionClause.From(Action<IConventionByScan> buildScanConvention)
        {
            var byScan = new ConventionByScanBuilder();
            buildScanConvention(byScan);
            _typeFinderBuilder = byScan;
            return this;
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
            var predicate = _predicates.ToPredicate();
            var typeFinder = _typeFinderBuilder.ToTypeFinder();
            return new ConventionServiceDefinition
                (predicate, typeFinder, _lifecycle ?? Lifecycle.Default, _precondition);
        }
    }
}
