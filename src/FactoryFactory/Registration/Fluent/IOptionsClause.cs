using System;

namespace FactoryFactory.Registration.Fluent
{
    public interface IOptionsClause
    {
        /// <summary>
        ///  Configures the lifecycle for this service.
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        IOptionsClause Lifecycle(ILifecycle lifecycle);

        /// <summary>
        ///  Sets a precondition for this service.
        /// </summary>
        /// <param name="precondition"></param>
        /// <returns></returns>
        IOptionsClause Precondition(Func<ServiceRequest, bool> precondition);
    }
}
