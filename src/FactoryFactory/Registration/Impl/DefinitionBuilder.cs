using System;
using System.Linq.Expressions;
using FactoryFactory.Registration.Fluent;
using FactoryFactory.Registration.ServiceDefinitions;

namespace FactoryFactory.Registration.Impl
{
    public class DefinitionBuilder
    {
        private protected Type _implementationType;
        private protected Expression<Func<ServiceRequest, object>> _implementationFactory;
        private protected object _implementationInstance;
        private protected DefinitionOptions _options = new DefinitionOptions();

        public DefinitionBuilder(Module module, Type type)
        {
            _implementationType = type;
            module.Add(() => Build(type));
        }

        private ServiceDefinition Build(Type type)
        {
            return new ServiceDefinition(type,
                implementationFactory: _implementationFactory,
                implementationType: _implementationType,
                implementationInstance: _implementationInstance,
                lifecycle: _options.Lifecycle, precondition: _options.Precondition);
        }

        /// <summary>
        ///  Specifies the concrete class to provide services for this definition.
        /// </summary>
        /// <param name="implementationType">
        ///  The concrete type to implement this service.
        /// </param>
        /// <returns></returns>
        public IOptionsClause As(Type implementationType)
        {
            _implementationFactory = null;
            _implementationInstance = null;
            _implementationType = implementationType;
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Specifies an already-created instance to provide services for this
        ///  definition.
        /// </summary>
        /// <param name="implementation">
        ///  The object that implements this service.
        /// </param>
        /// <returns></returns>
        public IOptionsClause As(object implementation)
        {
            _options.Lifecycle = Lifecycle.Untracked;
            _implementationFactory = null;
            _implementationInstance = implementation;
            _implementationType = null;
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this definition.
        /// </summary>
        /// <returns>
        ///  A factory method that creates the requested service.
        /// </returns>
        public IOptionsClause As(Expression<Func<ServiceRequest, object>> factory)
        {
            _implementationFactory = factory;
            _implementationInstance = null;
            _implementationType = null;
            return new OptionsBuilder(_options);
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
        public IOptionsClause As<TImplementation>()
            where TImplementation : TService
        {
            _implementationFactory = null;
            _implementationInstance = null;
            _implementationType = typeof(TImplementation);
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Specifies an already-created instance to provide services for this definition.
        /// </summary>
        /// <param name="implementation">
        ///  The object that implements this service.
        /// </param>
        /// <returns></returns>
        public IOptionsClause As(TService implementation)
        {
            _options.Lifecycle = Lifecycle.Untracked;
            _implementationInstance = implementation;
            _implementationFactory = null;
            _implementationType = null;
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this definition.
        /// </summary>
        /// <param name="factory">
        ///  A factory method that creates the requested service.
        /// </param>
        /// <returns></returns>
        public IOptionsClause As(Expression<Func<ServiceRequest, TService>> factory)
        {
            _implementationFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );
            _implementationType = null;
            _implementationInstance = null;
            return new OptionsBuilder(_options);
        }
    }
}
