using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FactoryFactory.Registration.Dsl
{
    public interface IInterceptionDefinitionClause<TService>
        : IOptionsClause<TService, IInterceptionDefinitionClause<TService>>
        where TService : class
    {
        /// <summary>
        ///  Specifies the concrete <see cref="IInterceptor{TService}"/> instance
        ///  to use to decorate services of type <see cref="TService"/>.
        /// </summary>
        /// <typeparam name="TImplementation">
        ///  The <see cref="IInterceptor{TService}"/> type which will decorate
        ///  this service.
        /// </typeparam>
        /// <returns></returns>
        Registry With<TImplementation>() where TImplementation : IInterceptor<TService>;

        /// <summary>
        ///  Specifies an already-created <see cref="IInterceptor{TService}"/>
        ///  instance to decorate services of type <see cref="TService"/>.
        /// </summary>
        /// <param name="implementation">
        ///  The object that will decorate this service.
        /// </param>
        /// <returns></returns>
        Registry With(IInterceptor<TService> implementation);

        /// <summary>
        ///  Specifies a factory expression to provide an
        ///  <see cref="IInterceptor{TService}"/> instance to decorate services
        ///  of type <see cref="TService"/>.
        /// </summary>
        /// <param name="factory">
        ///  A factory method that creates the requested service.
        /// </param>
        /// <returns></returns>
        Registry With(Expression<Func<ServiceRequest, IInterceptor<TService>>> factory);

        /// <summary>
        ///  Decorates all instances of <see cref="TService"/> using the
        ///  decorator function provided.
        /// </summary>
        /// <param name="decoratorFunc"></param>
        Registry By(Func<ServiceRequest, Func<TService>, TService> decoratorFunc);

        /// <summary>
        ///  Decorates all instances of <see cref="TService"/> using the
        ///  decorator function provided.
        /// </summary>
        /// <param name="decoratorFunc"></param>
        /// <returns></returns>
        Registry By(Func<Func<TService>, TService> decoratorFunc);
    }
}
