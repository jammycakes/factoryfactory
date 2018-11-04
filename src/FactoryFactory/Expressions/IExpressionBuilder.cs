using System;
using System.Linq.Expressions;
using System.Reflection;
using FactoryFactory.Impl;

namespace FactoryFactory.Compilation
{
    public interface IExpressionBuilder
    {
        Expression<Func<ServiceRequest, object>>
            CreateResolutionExpressionFromDefaultConstructor(ConstructorInfo selectedConstructor);

        Expression<Func<ServiceRequest, object>>
            CreateResolutionExpressionFromConstructorExpression(NewExpression nex);
    }
}
