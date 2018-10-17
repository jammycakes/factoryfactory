using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nooshka.Impl
{
    public class ResolverBuilder
    {
        private readonly ServiceDefinition _serviceDefinition;
        private readonly Configuration _configuration;

        public ResolverBuilder(ServiceDefinition serviceDefinition, Configuration configuration)
        {
            _serviceDefinition = serviceDefinition;
            _configuration = configuration;
        }

        private Expression<Func<ServiceRequest, object>> MakeExpression(
            Expression<Func<ServiceRequest, object>> req)
        {
            return req;
        }

        public ConstructorInfo GetBestConstructor()
        {
            var constructors = _serviceDefinition.ImplementationType.GetConstructors();
            var matchingConstructors =
                from constructor in constructors
                let parameters = constructor.GetParameters()
                let info = new { constructor, parameters, parameters.Length }
                where parameters.All(p => _configuration.IsTypeRegistered(p.ParameterType))
                orderby info descending
                select info.constructor;

            return matchingConstructors.First();
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

        public Resolver Build()
        {
            if (_serviceDefinition.ImplementationFactory != null) {
                return new RegistrationResolver(_serviceDefinition);
            }
            else {
                var constructor = GetBestConstructor();
                var expression = CreateServiceResolutionExpression(constructor);
                return new ExpressionResolver(_serviceDefinition, expression);
            }
        }
    }
}
