using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FactoryFactory.Expressions
{
    public interface IExpressionBuilder
    {
        Expression<Func<ServiceRequest, object>>
            CreateResolutionExpressionFromDefaultConstructor(ConstructorInfo selectedConstructor);

        Expression<Func<ServiceRequest, object>>
            CreateResolutionExpressionFromConstructorExpression(NewExpression nex);
    }
}
