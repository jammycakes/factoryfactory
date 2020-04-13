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
        public bool AutoResolve { get; set; }

        /// <summary>
        ///  The mechanism by which constructors are selected.
        /// </summary>
        public IConstructorSelector ConstructorSelector { get; set; }

        /// <summary>
        ///  The default lifecycle to apply to unregistered types.
        /// </summary>
        public ILifecycle DefaultLifecycle { get; set; }

        /// <summary>
        ///  The class used to create expressions to resolve dependencies.
        /// </summary>
        public IExpressionBuilder ExpressionBuilder { get; set; }

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
        ///  true if we are to auto-resolve unregistered types, otherwise false.
        /// </param>
        public ConfigurationOptions(
            IConstructorSelector constructorSelector = null,
            IExpressionBuilder expressionBuilder = null,
            ILifecycle defaultLifecycle = null,
            bool autoResolve = false
        )
        {
            AutoResolve = autoResolve;
            ConstructorSelector = constructorSelector ?? new DefaultConstructorSelector();
            DefaultLifecycle = defaultLifecycle ?? Lifecycle.Default;
            ExpressionBuilder = expressionBuilder ?? new ExpressionBuilder();
        }
    }
}
