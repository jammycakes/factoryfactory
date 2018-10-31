using FactoryFactory.Compilation;
using FactoryFactory.Compilation.Expressions;

namespace FactoryFactory
{
    public class ConfigurationOptions
    {
        public bool AutoResolve { get; }
        public IConstructorSelector ConstructorSelector { get; }
        public ILifecycle DefaultLifecycle { get; }
        public ICompiler Compiler { get; }

        public ConfigurationOptions(
            IConstructorSelector constructorSelector = null,
            ICompiler compiler = null,
            Lifecycle defaultLifecycle = null,
            bool autoResolve = true
        )
        {
            AutoResolve = autoResolve;
            ConstructorSelector = constructorSelector ?? new DefaultConstructorSelector();
            DefaultLifecycle = defaultLifecycle ?? Lifecycle.Default;
            Compiler = compiler ?? new ExpressionCompiler();
        }
    }
}
