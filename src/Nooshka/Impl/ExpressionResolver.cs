using System;
using System.Linq.Expressions;

namespace Nooshka.Impl
{
    public class ExpressionResolver : Resolver
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
