using System;

namespace Nooshka.Tests.Model
{
    public class ServiceWithFuncDependencies : IServiceWithDependencies
    {
        private readonly Func<IServiceWithoutDependencies> _dependencyFunc;
        public IServiceWithoutDependencies Dependency => _dependencyFunc();
        public string Message { get; }

        public ServiceWithFuncDependencies(Func<IServiceWithoutDependencies> dependencyFunc)
        {
            _dependencyFunc = dependencyFunc;
        }
    }
}
