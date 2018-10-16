using System;
using System.Linq.Expressions;
using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class ExpressionServiceResolver : ServiceResolver
    {
        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceResolver
            (ServiceRegistration registration, Expression<Func<ServiceRequest, object>> expression)
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
