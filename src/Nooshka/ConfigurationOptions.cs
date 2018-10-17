using Nooshka.Compilation;
using Nooshka.Compilation.Expressions;

namespace Nooshka
{
    public class ConfigurationOptions
    {
        public IConstructorSelector ConstructorSelector { get; }
        public IResolverCompiler ResolverCompiler { get; }

        public ConfigurationOptions(
            IConstructorSelector constructorSelector = null,
            IResolverCompiler resolverCompiler = null
        )
        {
            ConstructorSelector = constructorSelector ?? new DefaultConstructorSelector();
            ResolverCompiler = resolverCompiler ?? new ExpressionResolverCompiler();
        }
    }
}
