using System.Linq;
using System.Reflection;

namespace Nooshka.Compilation
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo SelectConstructor(ServiceDefinition serviceDefinition, Configuration configuration)
        {
            var constructors = serviceDefinition.ImplementationType.GetConstructors();
            var matchingConstructors =
                from constructor in constructors
                let parameters = constructor.GetParameters()
                let info = new { constructor, parameters, parameters.Length }
                where parameters.All(p => configuration.IsTypeRegistered(p.ParameterType))
                orderby info descending
                select info.constructor;

            return matchingConstructors.First();
        }
    }
}
