using System;
using System.Linq;
using System.Reflection;
using FactoryFactory.Util;

namespace FactoryFactory.Expressions
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo SelectConstructor(Type implementationType, Configuration configuration)
        {
            var constructors = implementationType.GetConstructors();
            var matchingConstructors =
                from constructor in constructors
                let parameters = constructor.GetParameters()
                let info = new {
                    constructor,
                    parameters,
                    parameters.Length,
                    funcParameterCount = parameters.Count(p => p.ParameterType.IsFunc())
                }
                where parameters.All(p =>
                    p.IsOptional ||
                    p.ParameterType.IsEnumerable() ||
                    configuration.CanResolveNew(p.ParameterType)
                )
                orderby info.Length descending, info.funcParameterCount descending
                select info.constructor;
            return matchingConstructors.FirstOrDefault();
        }
    }
}
