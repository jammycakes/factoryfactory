using System.Xml.Schema;
using Xunit;
using FactoryFactory.Util;
using FactoryFactory.Tests.Model;

namespace FactoryFactory.Tests.Util.TypeExtensions
{
    public class MakeGenericTypeFixture
    {
        private interface IBlueType<T> where T : IBlueService { }

        private class BlueService : IBlueService
        {
            public bool Intercepted { get; set; }
        }

        [Fact]
        public void CanMakeGenericTypeWhenConstraintsAreSatisfied()
        {
            var baseType = typeof(IBlueType<>);
            Assert.True(baseType.TryMakeGenericType(new[] {typeof(IBlueService)}, false, out var result));
            Assert.Equal(typeof(IBlueType<IBlueService>), result);
        }


        [Fact]
        public void CanMakeGenericTypeWhenConstraintsAreSatisfiedWithImplementation()
        {
            var baseType = typeof(IBlueType<>);
            Assert.True(baseType.TryMakeGenericType(new[] {typeof(BlueService)}, false, out var result));
            Assert.Equal(typeof(IBlueType<BlueService>), result);
        }

        [Fact]
        public void DoesNotMakeGenericTypeWhenConstraintsAreNotSatisfied()
        {
            var baseType = typeof(IBlueType<>);
            Assert.False(baseType.TryMakeGenericType(new[] {typeof(IGreenService)}, false, out var result));
            Assert.Null(result);
        }

        private interface IClassType<T> where T : class { }

        [Fact]
        public void MakesGenericTypeWhenClassConstraintIsSatisfied()
        {
            var baseType = typeof(IClassType<>);
            Assert.True(baseType.TryMakeGenericType(new[] {typeof(string)}, false, out var result));
            Assert.Equal(typeof(IClassType<string>), result);
        }

        [Fact]
        public void DoesNotMakeGenericTypeWhenClassConstraintIsNotSatisfied()
        {
            var baseType = typeof(IClassType<>);
            Assert.False(baseType.TryMakeGenericType(new[] {typeof(int)}, false, out var result));
            Assert.Null(result);
        }


        private interface IStructType<T> where T : struct { }

        [Fact]
        public void MakesGenericTypeWhenStructConstraintIsSatisfied()
        {
            var baseType = typeof(IStructType<>);
            Assert.True(baseType.TryMakeGenericType(new[] {typeof(int)}, false, out var result));
            Assert.Equal(typeof(IStructType<int>), result);
        }

        [Fact]
        public void DoesNotMakeGenericTypeWhenStructConstraintIsNotSatisfied()
        {
            var baseType = typeof(IStructType<>);
            Assert.False(baseType.TryMakeGenericType(new[] {typeof(string)}, false, out var result));
            Assert.Null(result);
        }



        private interface INewType<T> where T : new() { }

        public class WithNew
        {
            public WithNew() {}
        }

        public class WithoutNew
        {
            private WithoutNew() {}
        }

        public struct WithNewStruct
        {
        }

        [Fact]
        public void MakesGenericTypeWhenNewConstraintIsSatisfied()
        {
            var baseType = typeof(INewType<>);
            Assert.True(baseType.TryMakeGenericType(new[] {typeof(WithNew)}, false, out var result));
            Assert.Equal(typeof(INewType<WithNew>), result);
        }

        [Fact]
        public void DoesNotMakeGenericTypeWhenNewConstraintIsNotSatisfied()
        {
            var baseType = typeof(INewType<>);
            Assert.False(baseType.TryMakeGenericType(new[] {typeof(WithoutNew)}, false, out var result));
            Assert.Null(result);
        }


        [Fact]
        public void MakesGenericTypeWhenNewConstraintIsSatisfiedWithStruct()
        {
            var baseType = typeof(INewType<>);
            Assert.True(baseType.TryMakeGenericType(new[] {typeof(WithNewStruct)}, false, out var result));
            Assert.Equal(typeof(INewType<WithNewStruct>), result);
        }
    }
}
