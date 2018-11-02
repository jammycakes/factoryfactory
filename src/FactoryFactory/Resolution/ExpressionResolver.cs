using System;
using System.Linq.Expressions;
using FactoryFactory.Util;

namespace FactoryFactory.Resolution
{
    public class ExpressionResolver : IResolver
    {
        private readonly object _key = new object();
        private readonly Func<ServiceRequest, object> _func;

        public ExpressionResolver(IServiceDefinition definition, Expression<Func<ServiceRequest, object>> expression)
        {
            Priority = definition.Priority;
            _func = expression.CompileFast(true) ?? expression.Compile();
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public object Key => _key;

        public int Priority { get; }

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request) => _func(request);
    }
}
