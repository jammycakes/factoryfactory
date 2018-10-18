using System;
using System.Linq.Expressions;
using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class ExpressionServiceBuilder : ServiceBuilder
    {
        public Expression<Func<ServiceRequest, object>> Expression { get; }

        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceBuilder
            (ServiceDefinition definition, Expression<Func<ServiceRequest, object>> expression)
            : base(definition)
        {
            Expression = expression;
            _getService = expression.Compile();
        }

        protected override object Resolve(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
