using System;

namespace FactoryFactory.Tests.Model
{
    public class RecursiveServiceWithFuncDependency
    {
        private readonly Func<RecursiveServiceWithFuncDependency> _dependency;

        public RecursiveServiceWithFuncDependency Dependency => _dependency();

        public RecursiveServiceWithFuncDependency(Func<RecursiveServiceWithFuncDependency> dependency)
        {
            _dependency = dependency;
        }
    }
}
