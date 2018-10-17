using System;
using System.Linq.Expressions;
using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class ExpressionServiceBuilder : ServiceBuilder
    {
        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceBuilder
            (ServiceDefinition definition, Expression<Func<ServiceRequest, object>> expression)
            : base(definition)
        {
            _getService = expression.Compile();
        }

        protected override object Resolve(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
