using Logic.Entities;
using Xunit;

namespace Tests
{
    public class VariableTests
    {
        [Theory]
        [InlineData("x", "x")]
        [InlineData("x^0", "")]
        [InlineData("x^1", "x")]
        [InlineData("x^2", "x^2")]
        [InlineData("x^-1", "x^-1")]
        public void ParseVariable__ShouldBeOk(string input, string expected)
        {
            var parseResult = Variable.TryParse(input, out var variable);

            Assert.True(parseResult.IsSuccessfull, parseResult.GetErrorsString());
            Assert.Equal(expected, variable.ToString());
        }


        [Theory]
        [InlineData("x", "x", "x^2")]
        [InlineData("x^3", "x^3", "x^6")]
        public void MultiplyVariables__ShouldLeadToRightResult(string left, string right, string expected)
        {
            Variable.TryParse(left, out var leftVariable);
            Variable.TryParse(right, out var rightVariable);

            var result = leftVariable * rightVariable;

            Assert.Equal(expected, result.ToString());
        }
    }
}
