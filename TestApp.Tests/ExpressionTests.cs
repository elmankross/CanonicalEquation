using TestApp.Math;
using Xunit;

namespace TestApp.Tests
{
    public class ExpressionTests
    {
        [Theory]
        [InlineData("1", "1")]
        [InlineData("x", "x")]
        [InlineData("x^2", "x^2")]
        [InlineData("x^2y^2", "x^2y^2")]
        [InlineData("xyz", "xyz")]
        [InlineData("1.2x", "1.2x")]
        public void ParseExpression__ShouldBeOk(string input, string result)
        {
            var parseResult = AlgebraicExpression.TryParse(input, out var expression, out _);

            Assert.True(parseResult);
            Assert.Equal(result, expression.ToString());
        }
    }
}
