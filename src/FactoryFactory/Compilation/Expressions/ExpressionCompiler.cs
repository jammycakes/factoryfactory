using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FactoryFactory.Impl;

namespace FactoryFactory.Compilation.Expressions
{
    public class ExpressionCompiler : ICompiler
    {
        public IServiceBuilder Build(ServiceDefinition definition, Configuration configuration)
        {
            if (definition.ImplementationFactory != null) {
                if (definition.ImplementationFactory.Body is NewExpression nex) {
                    var expression = CreateExpressionFromConstructorExpression(nex);
                    return new ExpressionServiceBuilder(expression);
                }
                else {
                    return new ExpressionServiceBuilder(definition.ImplementationFactory);
                }
            }
            else {
                var constructor = configuration.Options.ConstructorSelector
                    .SelectConstructor(definition, configuration);
                if (constructor == null) return null;
                var expression = CreateExpressionFromDefaultConstructor(constructor);
                return new ExpressionServiceBuilder(expression);
            }
        }

        private Expression GetParameterResolutionExpression(Type type, ParameterExpression req)
        {
            return Expression.Convert(
                Expression.Call(
                    req,
                    typeof(ServiceRequest).GetMethod(nameof(ServiceRequest.ResolveDependency)),
                    Expression.Constant(type)
                ),
                type
            );
        }

        private Expression<Func<ServiceRequest, object>>
            CreateExpressionFromDefaultConstructor(ConstructorInfo selectedConstructor)
        {
            var req = Expression.Parameter(typeof(ServiceRequest), "serviceRequest");

            // Expression for each type looks like this:
            // (type)req => req.ResolveDependency(type)

            var constructorParameterExpressions =
                from parameter in selectedConstructor.GetParameters()
                select GetParameterResolutionExpression(parameter.ParameterType, req);

            return Expression.Lambda<Func<ServiceRequest, object>>(
                Expression.New(selectedConstructor, constructorParameterExpressions),
                req
            );
        }


        private Expression<Func<ServiceRequest, object>>
            CreateExpressionFromConstructorExpression(NewExpression nex)
        {
            var req = Expression.Parameter(typeof(ServiceRequest), "serviceRequest");
            var arguments =
                nex.Arguments.Select(x => MapConstructorArgument(x, req));
            var newExpression = nex.Members != null
                ? Expression.New(nex.Constructor, arguments, nex.Members)
                : Expression.New(nex.Constructor, arguments);
            return Expression.Lambda<Func<ServiceRequest, object>>(newExpression, req);
        }

        private Expression MapConstructorArgument(Expression expression, ParameterExpression req)
        {
            if (!(expression is MethodCallExpression mex)) return expression;
            if (mex.Method.DeclaringType != typeof(Resolve)) return expression;
            if (!mex.Method.IsGenericMethod) return expression;
            if (mex.Method.GetGenericMethodDefinition() !=
                typeof(Resolve).GetMethod(nameof(Resolve.From))) {
                return expression;
            }
            var type = mex.Method.GetGenericArguments().Single();
            return GetParameterResolutionExpression(type, req);
        }
    }
}
