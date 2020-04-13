using System;
using System.Collections.Generic;
using System.Text;
using FactoryFactory.Registration.Dsl.Descriptors;
using FactoryFactory.Registration.Fluent;
using FactoryFactory.Registration.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl
{
    public class ConventionDefinitionBuilder : IConventionDefinitionClause
    {
        private readonly Registry _registry;

        private ConventionPredicates _predicates = new ConventionPredicates();
        private ILifecycle _lifecycle = null;
        private ServiceLifetime _lifetime = default;
        private Func<ServiceRequest, bool> _precondition = null;
        private ITypeFinderBuilder _typeFinderBuilder;

        public ConventionDefinitionBuilder(Registry registry, Action<IConventionPredicates> predicates)
        {
            _registry = registry;
        }

        public IConventionDefinitionClause Lifecycle(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
            return this;
        }

        public IConventionDefinitionClause Lifecycle(ServiceLifetime lifetime)
        {
            _lifecycle = FactoryFactory.Lifecycle.Get(lifetime);
            _lifetime = lifetime;
            return this;
        }

        public IConventionDefinitionClause Singleton() => Lifecycle(ServiceLifetime.Singleton);

        public IConventionDefinitionClause Scoped() => Lifecycle(ServiceLifetime.Scoped);

        public IConventionDefinitionClause Transient() => Lifecycle(ServiceLifetime.Transient);

        public IConventionDefinitionClause Untracked() =>
            Lifecycle(FactoryFactory.Lifecycle.Untracked);

        public IConventionDefinitionClause Precondition(Func<ServiceRequest, bool> precondition)
        {
            _precondition = precondition;
            return this;
        }

        public Registry As(Action<IConventionByName> buildNameConvention)
        {
            var convention = new ConventionByNameBuilder();
            buildNameConvention(convention);
            _typeFinderBuilder = convention;
            AddConvention();
            return _registry;
        }

        public Registry Scanning(Action<IConventionByScan> buildScanConvention)
        {
            var convention = new ConventionByScanBuilder();
            buildScanConvention(convention);
            _typeFinderBuilder = convention;
            AddConvention();
            return _registry;
        }


        private void AddConvention()
        {
            var descriptor = new ConventionServiceDescriptor(
                _predicates.ToPredicate(),
                _typeFinderBuilder.ToTypeFinder(),
                _lifecycle,
                _lifetime,
                _precondition);
            _registry.Add(descriptor);
        }
    }
}
