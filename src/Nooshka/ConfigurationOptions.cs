using Nooshka.Compilation;
using Nooshka.Compilation.Expressions;

namespace Nooshka
{
    public class ConfigurationOptions
    {
        public bool AutoResolve { get; }
        public IConstructorSelector ConstructorSelector { get; }
        public Lifecycle DefaultLifecycle { get; }
        public IResolverCompiler ResolverCompiler { get; }

        public ConfigurationOptions(
            IConstructorSelector constructorSelector = null,
            IResolverCompiler resolverCompiler = null,
            Lifecycle defaultLifecycle = null,
            bool autoResolve = true
        )
        {
            AutoResolve = autoResolve;
            ConstructorSelector = constructorSelector ?? new DefaultConstructorSelector();
            DefaultLifecycle = defaultLifecycle ?? Lifecycle.Default;
            ResolverCompiler = resolverCompiler ?? new ExpressionResolverCompiler();
        }
    }
}
