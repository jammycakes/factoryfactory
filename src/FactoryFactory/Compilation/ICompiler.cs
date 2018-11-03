using System;
using System.Linq.Expressions;
using System.Reflection;
using FactoryFactory.Impl;

namespace FactoryFactory.Compilation
{
    public interface ICompiler
    {
        Expression<Func<ServiceRequest, object>>
            CreateExpressionFromDefaultConstructor(ConstructorInfo selectedConstructor);

        Expression<Func<ServiceRequest, object>>
            CreateExpressionFromConstructorExpression(NewExpression nex);
    }
}
