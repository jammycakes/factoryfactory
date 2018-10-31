using System;
using FactoryFactory.Impl;
using FactoryFactory.Util;
using SLE = System.Linq.Expressions;

namespace FactoryFactory.Compilation.Expressions
{
    public class ExpressionServiceBuilder : IServiceBuilder
    {
        public SLE.Expression<Func<ServiceRequest, object>> Expression { get; }

        private Func<ServiceRequest, object> _getService;

        public ExpressionServiceBuilder(SLE.Expression<Func<ServiceRequest, object>> expression)
        {
            Expression = expression;
            _getService = expression.CompileFast();
        }

        public object GetService(ServiceRequest request)
        {
            return _getService(request);
        }
    }
}
