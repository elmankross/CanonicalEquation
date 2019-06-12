using TestApp.Math;
using Xunit;

namespace TestApp.Tests
{
    public class VariableTests
    {
        [Theory]
        [InlineData('x', 0, "x^0")]
        [InlineData('x', 1, "x")]
        [InlineData('x', 2, "x^2")]
        public void CreateVariable__ShouldBeOk(char name, int power, string stringRepresentetion)
        {
            var variable = new AlgebraicVariable(name, power);

            Assert.Equal(stringRepresentetion, variable.ToString());
        }
    }
}
