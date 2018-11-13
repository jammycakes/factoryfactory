using System;
using System.Linq.Expressions;

namespace FactoryFactory.Registration
{
    public class InterceptionBuilder<TService> where TService : class
    {
        private Type _implementationType;
        private Expression<Func<ServiceRequest, object>> _implementationFactory;
        private object _implementationInstance;
        private readonly DefinitionOptions _options = new DefinitionOptions();

        public InterceptionBuilder(Module module)
        {
            module.Add(() =>
                new ServiceDefinition(typeof(IInterceptor<TService>),
                    implementationFactory: _implementationFactory,
                    implementationType: _implementationType,
                    implementationInstance: _implementationInstance,
                    lifecycle: _options.Lifecycle, precondition: _options.Precondition)
            );
        }

        /// <summary>
        ///  Specifies the concrete <see cref="IInterceptor{TService}"/> instance
        ///  to use to decorate services of type <see cref="TService"/>.
        /// </summary>
        /// <typeparam name="TImplementation">
        ///  The <see cref="IInterceptor{TService}"/> type which will decorate
        ///  this service.
        /// </typeparam>
        /// <returns></returns>
        public OptionsBuilder With<TImplementation>()
            where TImplementation : IInterceptor<TService>
        {
            _implementationType = typeof(TImplementation);
            _implementationFactory = null;
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Specifies an already-created <see cref="IInterceptor{TService}"/>
        ///  instance to decorate services of type <see cref="TService"/>.
        /// </summary>
        /// <param name="implementation">
        ///  The object that will decorate this service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder With(IInterceptor<TService> implementation)
        {
            _implementationFactory = null;
            _implementationType = null;
            _implementationInstance = implementation;
            _options.Lifecycle = Lifecycle.Untracked;
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Specifies a factory expression to provide an
        ///  <see cref="IInterceptor{TService}"/> instance to decorate services
        ///  of type <see cref="TService"/>.
        /// </summary>
        /// <param name="factory">
        ///  A factory method that creates the requested service.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder With(Expression<Func<ServiceRequest, IInterceptor<TService>>> factory)
        {
            _implementationFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );
            _implementationType = null;
            return new OptionsBuilder(_options);
        }

        /// <summary>
        ///  Decorates all instances of <see cref="TService"/> using the
        ///  decorator function provided.
        /// </summary>
        /// <param name="decoratorFunc"></param>
        public OptionsBuilder By(Func<ServiceRequest, Func<TService>, TService> decoratorFunc)
        {
            return With(new Interceptor<TService>(decoratorFunc)).Untracked();
        }

        /// <summary>
        ///  Decorates all instances of <see cref="TService"/> using the
        ///  decorator function provided.
        /// </summary>
        /// <param name="decoratorFunc"></param>
        /// <returns></returns>
        public OptionsBuilder By(Func<Func<TService>, TService> decoratorFunc)
        {
            return With(new Interceptor<TService>((req, svc) => decoratorFunc(svc))).Untracked();
        }
    }
}
