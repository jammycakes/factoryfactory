using System.Linq;
using Nooshka.Resolution;
using Xunit;


namespace Nooshka.Tests.Resolution.ResolverBuilderTests
{
    public class ExpressionFixture
    {
        [Fact]
        public void CanCreateServiceResolutionExpression()
        {
            var builder = new ResolverBuilder(null, null);

            var expr = builder.CreateServiceResolutionExpression(
                typeof(Container).GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .Last()
            );

            Assert.NotNull(expr);
        }
    }
}
