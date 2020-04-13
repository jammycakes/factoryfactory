using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl.Descriptors
{
    public class ConventionServiceDescriptor : ServiceDescriptor, IServiceDefinition
    {
        private readonly Predicate<Type> _predicate;
        private readonly Func<Type, IEnumerable<Type>> _typeFinder;
        private ILifecycle _lifecycle;

        public ConventionServiceDescriptor(
            Predicate<Type> predicate,
            Func<Type, IEnumerable<Type>> typeFinder,
            ILifecycle lifecycle,
            ServiceLifetime lifetime,
            Func<ServiceRequest, bool> precondition)
            : base(typeof(Dummy), svc => Dummy.Instance, lifetime)
        {
            _predicate = predicate;
            _typeFinder = typeFinder;
            _lifecycle = lifecycle;
            Precondition = precondition;
        }

        public IEnumerable<Type> GetTypes(Type requestedType)
            => _predicate(requestedType) ? _typeFinder(requestedType) : Type.EmptyTypes;

        public IEnumerable<object> GetInstances(Type requestedType)
            => Enumerable.Empty<object>();

        public IEnumerable<Expression<Func<ServiceRequest, object>>> GetExpressions
            (Type requestedType)
            => Enumerable.Empty<Expression<Func<ServiceRequest, object>>>();

        public Func<ServiceRequest, bool> Precondition { get; }

        public ILifecycle Lifecycle => _lifecycle ?? FactoryFactory.Lifecycle.Get(Lifetime);

        int IServiceDefinition.Priority => 1000;

        private class Dummy
        {
            public static readonly Dummy Instance = new Dummy();
        }
    }
}
