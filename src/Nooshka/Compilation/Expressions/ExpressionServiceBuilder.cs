using System;
using System.Linq.Expressions;
using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class ExpressionServiceBuilder : IServiceBuilder
    {
        public Expression<Func<ServiceRequest, object>> Expression { get; }

        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceBuilder(Expression<Func<ServiceRequest, object>> expression)
        {
            Expression = expression;
            _getService = expression.Compile();
        }

        public object GetService(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
