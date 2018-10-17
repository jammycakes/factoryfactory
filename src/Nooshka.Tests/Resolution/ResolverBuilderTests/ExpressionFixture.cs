using System.Linq;
using Nooshka.Compilation.Expressions;
using Nooshka.Impl;
using Xunit;


namespace Nooshka.Tests.Resolution.ResolverBuilderTests
{
    public class ExpressionFixture
    {
        [Fact]
        public void CanCreateServiceResolutionExpression()
        {
            var builder = new ExpressionResolverCompiler();

            var expr = builder.CreateServiceResolutionExpression(
                typeof(Container).GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .Last()
            );

            Assert.NotNull(expr);
        }
    }
}
