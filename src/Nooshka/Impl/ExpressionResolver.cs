using System;
using System.Linq.Expressions;

namespace Nooshka.Impl
{
    public class ExpressionResolver : Resolver
    {
        private Func<ServiceRequest, object> _getService;

        public ExpressionResolver
            (Registration registration, Expression<Func<ServiceRequest, object>> expression)
            : base(registration)
        {
            _getService = expression.Compile();
        }

        public override object GetService(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
