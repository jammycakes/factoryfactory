using FactoryFactory.Compilation;

namespace FactoryFactory
{
    public class ConfigurationOptions
    {
        public bool AutoResolve { get; }
        public IConstructorSelector ConstructorSelector { get; }
        public ILifecycle DefaultLifecycle { get; }
        public IExpressionBuilder ExpressionBuilder { get; }

        public ConfigurationOptions(
            IConstructorSelector constructorSelector = null,
            IExpressionBuilder expressionBuilder = null,
            ILifecycle defaultLifecycle = null,
            bool autoResolve = true
        )
        {
            AutoResolve = autoResolve;
            ConstructorSelector = constructorSelector ?? new DefaultConstructorSelector();
            DefaultLifecycle = defaultLifecycle ?? Lifecycle.Default;
            ExpressionBuilder = expressionBuilder ?? new ExpressionBuilder();
        }
    }
}
