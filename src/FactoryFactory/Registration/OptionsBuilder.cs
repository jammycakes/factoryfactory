using System;

namespace FactoryFactory.Registration
{
    public class OptionsBuilder
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
        public OptionsBuilder Lifecycle(ILifecycle lifecycle)
        {
            _options.Lifecycle = lifecycle;
            return this;
        }

        /// <summary>
        ///  Sets a precondition for this service.
        /// </summary>
        /// <param name="precondition"></param>
        /// <returns></returns>
        public OptionsBuilder Precondition(Func<ServiceRequest, bool> precondition)
        {
            _options.Precondition = precondition;
            return this;
        }

        /// <summary>
        ///  Configures the service as a singleton.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder Singleton() => Lifecycle(FactoryFactory.Lifecycle.Singleton);

        /// <summary>
        ///  Configures the service as a transient.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder Transient() => Lifecycle(FactoryFactory.Lifecycle.Transient);

        /// <summary>
        ///  Configures the service as a scoped service.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder Scoped() => Lifecycle(FactoryFactory.Lifecycle.Scoped);

        /// <summary>
        ///  Configures the service as an untracked service.
        ///  IDisposable.Dispose() will not be called when the container is
        ///  disposed.
        /// </summary>
        /// <returns></returns>
        public OptionsBuilder Untracked() => Lifecycle(FactoryFactory.Lifecycle.Untracked);
    }
}
