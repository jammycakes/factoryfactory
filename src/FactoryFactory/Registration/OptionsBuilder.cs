using System;

namespace FactoryFactory.Registration
{
    public class OptionsBuilder<TService>
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

        /// <summary>
        ///  Configures the service with a null lifecycle.
        ///  Multiple instances will be created per injection, and they will not
        ///  be tracked for disposal by any container.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> NullLifecycle() => Lifecycle(FactoryFactory.Lifecycle.Null);
    }
}
