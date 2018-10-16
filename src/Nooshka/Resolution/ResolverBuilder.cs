using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class ResolverBuilder
    {
        private readonly Registration.ServiceRegistration _serviceRegistration;
        private readonly ResolverCache _cache;

        public ResolverBuilder(Registration.ServiceRegistration serviceRegistration, ResolverCache cache)
        {
            _serviceRegistration = serviceRegistration;
            _cache = cache;
        }

        private Expression<Func<ServiceRequest, object>> MakeExpression(
            Expression<Func<ServiceRequest, object>> req)
        {
            return req;
        }

        public ConstructorInfo GetBestConstructor()
        {
            var constructors =
                from constructor in _serviceRegistration.ImplementationType.GetConstructors()
                let parameters = constructor.GetParameters()
                let info = new { constructor, parameters, parameters.Length }
                where parameters.All(p => _cache.IsTypeRegistered(p.ParameterType))
                orderby info descending
                select info.constructor;

            return constructors.Single();
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
            if (_serviceRegistration.ImplementationFactory != null) {
                return new RegistrationResolver(_serviceRegistration);
            }
            else {
                var constructor = GetBestConstructor();
                var expression = CreateServiceResolutionExpression(constructor);
                return new ExpressionResolver(_serviceRegistration, expression);
            }
        }
    }
}
