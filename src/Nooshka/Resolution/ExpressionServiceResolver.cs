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
            Registration.ServiceRegistration serviceRegistration,
            Expression<Func<ServiceRequest, object>> _getServiceExpression
        )
        {
            _preconditionMet = serviceRegistration.Precondition;
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
