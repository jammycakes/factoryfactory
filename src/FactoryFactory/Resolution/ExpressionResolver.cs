using System;
using System.Linq.Expressions;
using FactoryFactory.Util.FastExpressions;

namespace FactoryFactory.Resolution
{
    public class ExpressionResolver : IResolver
    {
        private readonly object _key = new object();
        private readonly Func<ServiceRequest, object> _func;

        public ExpressionResolver(IServiceDefinition definition, Expression<Func<ServiceRequest, object>> expression, Type type)
        {
            Priority = definition.Priority;
            _func = expression.CompileFast(true) ?? expression.Compile();
            Type = type;
        }

        public bool CanResolve => true;

        public bool Conditional => false;

        public object Key => _key;

        public int Priority { get; }

        public Type Type { get; }

        public bool IsConditionMet(ServiceRequest request) => true;

        public object GetService(ServiceRequest request) => _func(request);

        public override string ToString() => $"ExpressionResolver for {Type}";
    }
}
