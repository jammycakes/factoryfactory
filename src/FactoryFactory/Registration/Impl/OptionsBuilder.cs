using System;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory.Registration.Impl
{
    public class OptionsBuilder : IOptionsClause
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
        public IOptionsClause Lifecycle(ILifecycle lifecycle)
        {
            _options.Lifecycle = lifecycle;
            return this;
        }

        /// <summary>
        ///  Sets a precondition for this service.
        /// </summary>
        /// <param name="precondition"></param>
        /// <returns></returns>
        public IOptionsClause Precondition(Func<ServiceRequest, bool> precondition)
        {
            _options.Precondition = precondition;
            return this;
        }
    }
}
