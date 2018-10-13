using System;
using System.Linq.Expressions;
using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class ExpressionServiceResolver : IServiceResolver
    {
        private Func<ServiceRequest, bool> _preconditionMet;
        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceResolver(
            IRegistration registration,
            Expression<Func<ServiceRequest, object>> _getServiceExpression
        )
        {
            _preconditionMet = registration.Precondition;
            _getService = _getServiceExpression.Compile();
        }


        public bool PreconditionMet(ServiceRequest request)
        {
            return _preconditionMet(request);
        }

        public object GetService(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
