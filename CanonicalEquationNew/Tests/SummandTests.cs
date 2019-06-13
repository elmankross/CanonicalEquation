using Logic.Entities;
using Xunit;

namespace Tests
{
    public class SummandTests
    {
        [Theory]
        [InlineData("-", "-1")]
        [InlineData("xyz", "+xyz")]
        [InlineData("xxx", "+x^3")]
        [InlineData("zyx", "+xyz")]
        [InlineData("x^2y^2", "+x^2y^2")]
        [InlineData("x^1y^0", "+x")]
        [InlineData("xy^-1", "+xy^-1")]
        [InlineData("4.5xy", "+4,5xy")]
        [InlineData("-1.0zyyyx", "-xy^3z")]
        [InlineData("-(xyz)", "-(xyz)")]
        [InlineData("-(xyz-zyx)", "-(xyz-xyz)")]
        [InlineData("-x(xyz-zyx)", "-x(xyz-xyz)")]
        [InlineData("-x(y(xyz-zyx))", "-x(y(xyz-xyz))")]
        [InlineData("x(xyz-zyx(x-y))", "+x(xyz-xyz(x-y))")]
        public void ParseSummand__ShouldBeOk(string input, string expected)
        {
            var parseResult = Summand.TryParse(input, out var summand);

            Assert.True(parseResult.IsSuccessfull, parseResult.GetErrorsString());
            Assert.Equal(expected, summand.ToString());
        }
    }
}
