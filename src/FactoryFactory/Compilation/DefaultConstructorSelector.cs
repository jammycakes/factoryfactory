using System.Linq;
using System.Reflection;
using FactoryFactory.Util;

namespace FactoryFactory.Compilation
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo SelectConstructor(ServiceDefinition serviceDefinition, Configuration configuration)
        {
            var constructors = serviceDefinition.ImplementationType.GetConstructors();
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
                    configuration.CanResolve(p.ParameterType.GetServiceType())
                )
                orderby info.Length descending, info.funcParameterCount descending
                select info.constructor;
            return matchingConstructors.FirstOrDefault();
        }
    }
}
