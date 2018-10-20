using System;

namespace FactoryFactory.Impl
{
    public class OptionsBuilder<TService>
    {
        private readonly DefinitionOptions _options;

        public OptionsBuilder(DefinitionOptions options)
        {
            _options = options;
        }

        /// <summary>
        ///  Configures the lifecycle for this registration.
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public OptionsBuilder<TService> Lifecycle(Lifecycle lifecycle)
        {
            _options.Lifecycle = lifecycle;
            return this;
        }

        /// <summary>
        ///  Sets a precondition for this registration.
        /// </summary>
        /// <param name="precondition"></param>
        /// <returns></returns>
        public OptionsBuilder<TService> Precondition(Func<ServiceRequest, bool> precondition)
        {
            _options.Precondition = precondition;
            return this;
        }

        /// <summary>
        ///  Configures this registration as a singleton.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Singleton() => Lifecycle(FactoryFactory.Lifecycle.Singleton);

        /// <summary>
        ///  Configures this registration as a transient.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Transient() => Lifecycle(FactoryFactory.Lifecycle.Transient);

        /// <summary>
        ///  Configures this registration as a scoped service.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Scoped() => Lifecycle(FactoryFactory.Lifecycle.Scoped);

        /// <summary>
        ///  Configures this registration as an untracked service.
        ///  IDisposable.Dispose() will not be called when the container is
        ///  disposed.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> Untracked() => Lifecycle(FactoryFactory.Lifecycle.Untracked);

        /// <summary>
        ///  Configures this registration with a null lifecycle.
        ///  Multiple instances will be created per injection, and they will not
        ///  be tracked for disposal by any container.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder<TService> NullLifecycle() => Lifecycle(FactoryFactory.Lifecycle.Null);
    }
}
