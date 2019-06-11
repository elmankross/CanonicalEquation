using TestApp.Math;
using Xunit;

namespace TestApp.Tests
{
    public class ProgramTests
    {
        [Theory]
        [InlineData("1=1", "1=1")]
        [InlineData("x=y", "x=y")]
        [InlineData("x^2=x", "x^2=x")]
        [InlineData("x - 2 = 1", "x-2=1")]
        [InlineData("x-1=x-1", "x-1=x-1")]
        [InlineData("x-x(x-x)=y-y(y-y)", "x-x(x-x)=y-y(y-y)")]
        [InlineData("x - (x^2 - xy - 2) = 0", "x-(x^2-xy-2)=0")]
        [InlineData("x^2 + 3.5xy + y = y^2 - xy + y", "x^2+3.5xy+y=y^2-xy+y")]
        [InlineData("4a^2b - 2=2b^2a(a-1)", "4a^2b-2=2b^2a(a-1)")]
        [InlineData("x^2+ 83 + zxy = 4 - ds+2", "x^2+83+zxy=4-ds+2")]
        [InlineData("x - (x^0 - (0 - x + y - y^0)) = 0", "x-(x^0-(0-x+y-y^0))=0")]
        [InlineData("x^2 + 2xy + y^2 = 5y^2 - 3xy + 2y^2", "x^2+2xy+y^2=5y^2-3xy+2y^2")]
        public void ParseEquation__ShouldBeOk(string input, string result)
        {
            var parseResult = AlgebraicEquation.TryParse(input, out var equation, out _);

            Assert.True(parseResult);
            Assert.Equal(result, equation.ToString());
        }


        [Theory]
        [InlineData("x^2 + 3.5xy + y = y^2 - xy + y", "x^2-y^2+4.5xy=0")]
        [InlineData("x^2 + 2xy + y^2 = 5y^2 - 3xy + 2y^2", "-4x^2-y^2+5xy=0")]
        [InlineData("x - 2 = 1", "x-3=0")]
        [InlineData("x - (x^2 - xy - 2) = 0", "-x^2+xy+x+2=0")]
        [InlineData("x - (x^0 - (0 - x + y - y^0)) = 0", "y-2=0")]
        [InlineData("x^3 - y(x + (x-1)(y+3)) = 0", "x^3-y^2x+y^2-4xy+3y=0")]
        public void TranslateToCanonicalForm__ShouldHasRightResult(string input, string result)
        {
            var parseResult = AlgebraicEquation.TryParse(input, out var equation, out _);
            equation.ToCanonicalForm();

            Assert.True(parseResult);
            Assert.Equal(result, equation.ToString());
        }


        [Theory]
        [InlineData("", "No input data")]
        [InlineData("=", "Input string is not equation: input must contains '=' sign")]
        [InlineData("&", "Input string is not equation: input must contains '=' sign")]
        public void ParseInvalidExpression__ShouldBeFailed(string input, string result)
        {
            var parseResult = AlgebraicEquation.TryParse(input, out _, out var error);

            Assert.False(parseResult);
            Assert.Equal(result, error);
        }
    }
}
