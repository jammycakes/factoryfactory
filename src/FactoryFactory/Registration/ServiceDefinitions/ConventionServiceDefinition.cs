using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FactoryFactory.Registration.ServiceDefinitions
{
    public class ConventionServiceDefinition : IServiceDefinition
    {
        private readonly Predicate<Type> _predicate;
        private readonly Func<Type, IEnumerable<Type>> _typeFinder;
        private Func<ServiceRequest, bool> _precondition;
        private ILifecycle _lifecycle;

        public ConventionServiceDefinition(
            Predicate<Type> predicate,
            Func<Type,IEnumerable<Type>> typeFinder,
            ILifecycle lifecycle,
            Func<ServiceRequest,bool> precondition)
        {
            _predicate = predicate;
            _typeFinder = typeFinder;
            _lifecycle = lifecycle;
            _precondition = precondition;
        }


        IEnumerable<Type> IServiceDefinition.GetTypes(Type requestedType)
        {
            if (_predicate(requestedType)) {
                return _typeFinder(requestedType);
            }
            else {
                return Type.EmptyTypes;
            }
        }

        IEnumerable<object> IServiceDefinition.GetInstances(Type requestedType)
        {
            yield break;
        }

        IEnumerable<Expression<Func<ServiceRequest, object>>> IServiceDefinition.GetExpressions(Type requestedType)
        {
            yield break;
        }

        Func<ServiceRequest, bool> IServiceDefinition.Precondition => _precondition;

        ILifecycle IServiceDefinition.Lifecycle => _lifecycle;

        int IServiceDefinition.Priority => 1000;
    }
}
