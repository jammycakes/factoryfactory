using System;

namespace FactoryFactory.Registration
{
    public class OptionsBuilder<TService> where TService: class
    {
        private readonly DefinitionOptions _options;

        public OptionsBuilder(DefinitionOptions options)
        {
            _options = options;
        }

        /// <summary>
        ///  Configures the lifecycle for this service.
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public OptionsBuilder<TService> Lifecycle(Lifecycle lifecycle)
        {
            _options.Lifecycle = lifecycle;
            return this;
        }

        /// <summary>
        ///  Sets a precondition for this service.
        /// </summary>
        /// <param name="precondition"></param>
        /// <returns></returns>
        public OptionsBuilder<TService> Precondition(Func<ServiceRequest, bool> precondition)
        {
            _options.Precondition = precondition;
            return this;
        }

        /// <summary>
        ///  Proxies the resolved service with a decorator.
        /// </summary>
        /// <param name="decorator">
        ///  A function that takes the service request and the generated service
        ///  and returns a proxy in its place.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<TService> Proxy(Func<ServiceRequest, TService, TService> decorator)
        {
            _options.Decorator = (req, svc) => decorator(req, (TService)svc);
            return this;
        }

        /// <summary>
        ///  Proxies the resolved service with a decorator.
        /// </summary>
        /// <param name="decorator">
        ///  A function that takes the generated service and returns a proxy in
        ///  its place. Use this overload when you do not need to take the
        ///  service request information into account.
        /// </param>
        /// <returns></returns>
        public OptionsBuilder<TService> Proxy(Func<TService, TService> decorator)
        {
            _options.Decorator = (req, svc) => decorator((TService)svc);
            return this;
        }

        /// <summary>
        ///  Configures the service as a singleton.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Singleton() => Lifecycle(FactoryFactory.Lifecycle.Singleton);

        /// <summary>
        ///  Configures the service as a transient.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Transient() => Lifecycle(FactoryFactory.Lifecycle.Transient);

        /// <summary>
        ///  Configures the service as a scoped service.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Scoped() => Lifecycle(FactoryFactory.Lifecycle.Scoped);

        /// <summary>
        ///  Configures the service as an untracked service.
        ///  IDisposable.Dispose() will not be called when the container is
        ///  disposed.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Untracked() => Lifecycle(FactoryFactory.Lifecycle.Untracked);
    }
}
