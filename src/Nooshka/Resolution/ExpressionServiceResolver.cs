using System;
using System.Linq.Expressions;

namespace Nooshka.Resolution
{
    public class ExpressionServiceResolver : IServiceResolver
    {
        private Func<ServiceRequest, bool> _preconditionMet;
        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceResolver(
            Expression<Func<ServiceRequest, bool>> _preconditionMetExpression,
            Expression<Func<ServiceRequest, object>> _getServiceExpression
        )
        {
            _preconditionMet = _preconditionMetExpression.Compile();
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
