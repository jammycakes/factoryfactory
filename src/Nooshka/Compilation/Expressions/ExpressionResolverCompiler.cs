using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nooshka.Impl;

namespace Nooshka.Compilation.Expressions
{
    public class ExpressionResolverCompiler : IResolverCompiler
    {
        public IServiceBuilder Build(ServiceDefinition definition, Configuration configuration)
        {
            if (definition.ImplementationFactory != null) {
                return new RegistrationServiceBuilder(definition);
            }
            else {
                var constructor = configuration.Options.ConstructorSelector
                    .SelectConstructor(definition, configuration);
                var expression = CreateServiceResolutionExpression(constructor);
                return new ExpressionServiceBuilder(definition, expression);
            }
        }

        public Expression<Func<ServiceRequest, object>>
            CreateServiceResolutionExpression(ConstructorInfo selectedConstructor)
        {
            var req = Expression.Parameter(typeof(ServiceRequest), "serviceRequest");

            // Expression for each type looks like this:
            // (type)req => req.ResolveDependency(type)

            Expression GetParameterResolutionExpression(Type type)
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

            var constructorParameterExpressions =
                from parameter in selectedConstructor.GetParameters()
                select GetParameterResolutionExpression(parameter.ParameterType);

            return Expression.Lambda<Func<ServiceRequest, object>>(
                Expression.New(selectedConstructor, constructorParameterExpressions),
                req
            );
        }
    }
}
