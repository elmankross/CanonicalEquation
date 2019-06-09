using TestApp.Math;
using Xunit;

namespace TestApp.Tests
{
    public class ProgramTests
    {
        [Theory]
        [InlineData("", "Input string is not equation")]
        [InlineData("=", "Input string is not equation")]
        [InlineData("&", "Input string is not equation")]
        [InlineData("x^2+3.5xy+y=y^2-xy+y", "x^2-y^2+4.5xy=0")]
        public void GetCanonicalForm__ShouldHasRightResult(string input, string output)
        {
            Assert.Equal(output,
                AlgebraicEquation.TryParse(input, out var equation, out var error)
                    ? equation.GetCanonicalForm().ToString()
                    : error);
        }
    }
}
