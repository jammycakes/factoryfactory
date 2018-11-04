using FactoryFactory.Expressions;

namespace FactoryFactory
{
    /// <summary>
    ///  Contains some basic options that control the behaviour of the IOC
    ///  container.
    /// </summary>
    public class ConfigurationOptions
    {
        /// <summary>
        ///  Indicates that unregistered types should be automatically resolved
        ///  where possible.
        /// </summary>
        public bool AutoResolve { get; }

        /// <summary>
        ///  The mechanism by which constructors are selected.
        /// </summary>
        public IConstructorSelector ConstructorSelector { get; }

        /// <summary>
        ///  The default lifecycle to apply to unregistered types.
        /// </summary>
        public ILifecycle DefaultLifecycle { get; }

        /// <summary>
        ///  The class used to create expressions to resolve dependencies.
        /// </summary>
        public IExpressionBuilder ExpressionBuilder { get; }

        /// <summary>
        ///  Creates a new instance of the <see cref="ConfigurationOptions"/>
        ///  class.
        /// </summary>
        /// <param name="constructorSelector">
        ///  Indicates that unregistered types should be automatically resolved
        ///  where possible.
        /// </param>
        /// <param name="expressionBuilder">
        ///  The mechanism by which constructors are selected.
        /// </param>
        /// <param name="defaultLifecycle">
        ///  The default lifecycle to apply to unregistered types.
        /// </param>
        /// <param name="autoResolve">
        ///  The class used to create expressions to resolve dependencies.
        /// </param>
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
