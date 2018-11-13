using FactoryFactory.Registration.Fluent;

namespace FactoryFactory
{
    public static class FluentExtensions
    {
        /* ====== IOptionsClause ====== */


        /// <summary>
        ///  Configures the service as a singleton.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Singleton(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Singleton);

        /// <summary>
        ///  Configures the service as a transient.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Transient(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Transient);

        /// <summary>
        ///  Configures the service as a scoped service.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Scoped(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Scoped);

        /// <summary>
        ///  Configures the service as an untracked service.
        ///  IDisposable.Dispose() will not be called when the container is
        ///  disposed.
        /// </summary>
        /// <returns></returns>
        public static IOptionsClause Untracked(this IOptionsClause options)
            => options.Lifecycle(FactoryFactory.Lifecycle.Untracked);
    }
}
