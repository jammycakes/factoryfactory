using System;

namespace FactoryFactory.Tests.Model
{
    public class RecursiveServiceWithLazyDependency
    {
        private readonly Lazy<RecursiveServiceWithLazyDependency> _dependency;

        public RecursiveServiceWithLazyDependency Dependency => _dependency.Value;

        public RecursiveServiceWithLazyDependency(Lazy<RecursiveServiceWithLazyDependency> dependency)
        {
            _dependency = dependency;
        }
    }
}
