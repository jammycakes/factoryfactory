using System;
using System.ComponentModel.Design;
using System.Threading;
using SLE = System.Linq.Expressions;
using Nooshka.Impl;
using Nooshka.Util;

namespace Nooshka.Compilation.Expressions
{
    public class ExpressionServiceBuilder : IServiceBuilder
    {
        public SLE.Expression<Func<ServiceRequest, object>> Expression { get; }

        private Func<ServiceRequest, object> _getService;
        private Func<ServiceRequest, object> _getServiceFunc;

        public ExpressionServiceBuilder(SLE.Expression<Func<ServiceRequest, object>> expression,
            Type registeredType)
        {
            Expression = expression;
            _getService = expression.Compile();

            var lambdaType = typeof(Func<>).MakeGenericType(registeredType);
            var param = SLE.Expression.Parameter(typeof(ServiceRequest));
            var getServiceFuncExpr = SLE.Expression.Lambda<Func<ServiceRequest, object>>(
                SLE.Expression.Lambda(
                    lambdaType,
                    SLE.Expression.Convert(SLE.Expression.Invoke(expression, param), registeredType)
                ),
                param
            );
            _getServiceFunc = getServiceFuncExpr.Compile();
        }

        public object GetService(ServiceRequest request)
        {
            if (request.RequestedType.IsFunc()) {
                return _getServiceFunc(request);
            }
            else {
                return _getService(request);
            }
        }
    }
}
