using System;
using System.Linq.Expressions;
using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class ExpressionResolver : ResolverBase
    {
        private Func<ServiceRequest, object> _getService;

        public ExpressionResolver
            (ServiceDefinition serviceDefinition, Expression<Func<ServiceRequest, object>> expression)
            : base(serviceDefinition)
        {
            _getService = expression.Compile();
        }

        protected override object Resolve(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
