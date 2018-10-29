using System;
using System.Linq.Expressions;

namespace FactoryFactory.Registration
{
    public class DefinitionBuilder
    {
        private protected Type _implementationType;
        private protected Expression<Func<ServiceRequest, object>> _implementationFactory;
        private protected DefinitionOptions _options = new DefinitionOptions();

        public DefinitionBuilder(Module module, Type type)
        {
            module.Add(() => Build(type));
        }

        private ServiceDefinition Build(Type type)
        {
            return new ServiceDefinition(type,
                implementationType: _implementationType,
                implementationFactory: _implementationFactory,
                lifecycle: _options.Lifecycle,
                precondition: _options.Precondition
            );
        }

        /// <summary>
        ///  Specifies the concrete class to provide services for this definition.
        /// </summary>
        /// <param name="implementationType">
        ///  The concrete type to implement this service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<object> As(Type implementationType)
        {
            _implementationType = implementationType;
            return new OptionsBuilder<object>(_options);
        }

        /// <summary>
        ///  Specifies an already-created instance to provide services for this
        ///  definition.
        /// </summary>
        /// <param name="implementation">
        ///  The object that implements this service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<object> As(object implementation)
        {
            return As(req => implementation);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this definition.
        /// </summary>
        /// <returns>
        ///  A factory method that creates the requested service.
        /// </returns>
        public OptionsBuilder<object> As(Expression<Func<ServiceRequest, object>> factory)
        {
            _implementationFactory = factory;
            _implementationType = null;
            return new OptionsBuilder<object>(_options);
        }
    }

    public class DefinitionBuilder<TService> : DefinitionBuilder where TService: class
    {
        public DefinitionBuilder(Module module)
            : base(module, typeof(TService))
        {
        }

        /// <summary>
        ///  Specifies the concrete class to provide services for this definition.
        /// </summary>
        /// <returns></returns>
        /// <typeparam name="TImplementation">
        ///  The concrete type to implement this service.
        /// </typeparam>
        /// <returns></returns>
        public OptionsBuilder<TService> As<TImplementation>()
            where TImplementation : TService
        {
            _implementationType = typeof(TImplementation);
            _implementationFactory = null;
            return new OptionsBuilder<TService>(_options);
        }

        /// <summary>
        ///  Specifies an already-created instance to provide services for this definition.
        /// </summary>
        /// <param name="implementation">
        ///  The object that implements this service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<TService> As(TService implementation)
        {
            _implementationFactory = req => implementation;
            _implementationType = null;
            return new OptionsBuilder<TService>(_options);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this definition.
        /// </summary>
        /// <param name="factory">
        ///  A factory method that creates the requested service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<TService> As(Expression<Func<ServiceRequest, TService>> factory)
        {
            _implementationFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );
            _implementationType = null;
            return new OptionsBuilder<TService>(_options);
        }
    }
}
