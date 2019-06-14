using Logic.Entities;
using Xunit;

namespace Tests
{
    public class EquationTests
    {
        [Theory]
        [InlineData("", "Input is empty")]
        [InlineData("@^)_&*", "'@^)_&*': Equation should contains one '=' sign")]
        [InlineData("=", "Expression is empty")]
        [InlineData("x=x=x", "'x=x=x': Equation should contains one '=' sign")]
        public void ParseInvalidEquation__ShouldBeFailed(string input, string expectedError)
        {
            var parseResult = Equation.TryParse(input, out _);

            Assert.False(parseResult.IsSuccessfull);
            Assert.Equal(expectedError, parseResult.GetErrorsString());
        }


        [Theory]
        [InlineData("x^2 + 2xy + y^2 = 5y^2 - 3xy + 2y^2", "x^2+2xy+y^2=5y^2-3xy+2y^2")]
        [InlineData("x - 2 = 1", "x-2=1")]
        [InlineData("x - (x^2 - xy - 2) = 0", "x-(x^2-xy-2)=0")]
        [InlineData("x - (x^0 - (0 - x + y - y^0)) = 0", "x-(1-(0-x+y-1))=0")]
        public void ParseEquation__ShouldBeOk(string input, string expected)
        {
            var parseResult = Equation.TryParse(input, out var equation);

            Assert.True(parseResult.IsSuccessfull, parseResult.GetErrorsString());
            Assert.Equal(expected, equation.ToString());
        }


        [Theory]
        [InlineData("x^2 + 2xy + y^2 = 5y^2 - 3xy + 2y^2", "x^2-6y^2+5xy=0")]
        [InlineData("x - 2 = 1", "x-3=0")]
        [InlineData("x - (x^2 - xy - 2) = 0", "-x^2+xy+x+2=0")]
        [InlineData("x - (x^0 - (0 - x + y - y^0)) = 0", "y-2=0")]
        [InlineData("x - x(x^0 - 1) = 0", "x=0")]
        [InlineData("x - x(x^0 + xy + 1) = 0", "-x^2y-x=0")]
        public void ConvertToCanonicalForm__ShouldBeOk(string input, string expected)
        {
            var parseResult = Equation.TryParse(input, out var equation);
            equation = equation.GetCanonicalForm();

            Assert.True(parseResult.IsSuccessfull, parseResult.GetErrorsString());
            Assert.Equal(expected, equation.ToString());
        }


        [Theory]
        [InlineData("x-2", "x+1", "x-2-x-1")]
        [InlineData("xyz", "xy", "xyz-xy")]
        [InlineData("x(x-y)", "x(x-y)", "x(x-y)-x(x-y)")]
        public void SubtractExpression__ShouldLeadToRightResult(string leftExp, string rightExp, string expectedResult)
        {
            Expression.TryParse(leftExp, out var leftExpression);
            Expression.TryParse(rightExp, out var rightExpression);

            var resultExpression = leftExpression - rightExpression;
            Assert.Equal(expectedResult, resultExpression.ToString());
        }
    }
}
