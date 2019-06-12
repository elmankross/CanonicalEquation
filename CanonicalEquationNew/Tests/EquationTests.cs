using Logic.Entities;
using Xunit;

namespace Tests
{
    public class EquationTests
    {
        [Theory]
        [InlineData("x^2 + 2xy + y^2 = 5y^2 - 3xy + 2y^2", "-4x^2-y^2+5xy=0")]
        [InlineData("x - 2 = 1", "x-3=0")]
        [InlineData("x - (x^2 - xy - 2) = 0", "-x^2+xy+x+2=0")]
        [InlineData("x - (x^0 - (0 - x + y - y^0)) = 0", "y-2=0")]
        public void ParseEquation__ShouldBeOk(string input, string expected)
        {
            var parseResult = Equation.TryParse(input, out var equation);

            Assert.True(parseResult.IsSuccessfull, parseResult.GetErrorsString());
            Assert.Equal(expected, equation.ToString());
        }
    }
}
