using System;
using System.Linq.Expressions;

namespace FactoryFactory.Registration
{
    public class DecorationBuilder<TService> where TService : class
    {
        private Type _implementationType;
        private Expression<Func<ServiceRequest, object>> _implementationFactory;
        private readonly DefinitionOptions _options = new DefinitionOptions();

        public DecorationBuilder(Module module)
        {
            module.Add(() =>
                new ServiceDefinition(typeof(IDecorator<TService>),
                    implementationFactory: _implementationFactory,
                    implementationType: _implementationType,
                    lifecycle: _options.Lifecycle, precondition: _options.Precondition)
            );
        }

        /// <summary>
        ///  Specifies the concrete <see cref="IDecorator{TService}"/> instance
        ///  to use to decorate services of type <see cref="TService"/>.
        /// </summary>
        /// <typeparam name="TImplementation">
        ///  The <see cref="IDecorator{TService}"/> type which will decorate
        ///  this service.
        /// </typeparam>
        /// <returns></returns>
        public OptionsBuilder<IDecorator<TService>> With<TImplementation>()
            where TImplementation : IDecorator<TService>
        {
            _implementationType = typeof(TImplementation);
            _implementationFactory = null;
            return new OptionsBuilder<IDecorator<TService>>(_options);
        }

        /// <summary>
        ///  Specifies an already-created <see cref="IDecorator{TService}"/>
        ///  instance to decorate services of type <see cref="TService"/>.
        /// </summary>
        /// <param name="implementation">
        ///  The object that will decorate this service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<IDecorator<TService>> With(IDecorator<TService> implementation)
        {
            _implementationFactory = req => implementation;
            _implementationType = null;
            _options.Lifecycle = Lifecycle.Untracked;
            return new OptionsBuilder<IDecorator<TService>>(_options);
        }

        /// <summary>
        ///  Specifies a factory expression to provide an
        ///  <see cref="IDecorator{TService}"/> instance to decorate services
        ///  of type <see cref="TService"/>.
        /// </summary>
        /// <param name="factory">
        ///  A factory method that creates the requested service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<IDecorator<TService>> With(Expression<Func<ServiceRequest, IDecorator<TService>>> factory)
        {
            _implementationFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );
            _implementationType = null;
            return new OptionsBuilder<IDecorator<TService>>(_options);
        }

        /// <summary>
        ///  Decorates all instances of <see cref="TService"/> using the
        ///  decorator function provided.
        /// </summary>
        /// <param name="decoratorFunc"></param>
        public OptionsBuilder<IDecorator<TService>> By(Func<ServiceRequest, TService, TService> decoratorFunc)
        {
            return With(new Decorator<TService>(decoratorFunc)).Untracked();
        }

        /// <summary>
        ///  Decorates all instances of <see cref="TService"/> using the
        ///  decorator function provided.
        /// </summary>
        /// <param name="decoratorFunc"></param>
        /// <returns></returns>
        public OptionsBuilder<IDecorator<TService>> By(Func<TService, TService> decoratorFunc)
        {
            return With(new Decorator<TService>((req, svc) => decoratorFunc(svc))).Untracked();
        }
    }
}
