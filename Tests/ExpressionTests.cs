﻿using Logic.Entities;
using Xunit;

namespace Tests
{
    public class ExpressionTests
    {
        [Theory]
        [InlineData("x^2 + 2xy + y^2", "x^2+2xy+y^2")]
        [InlineData("5y^2 - 3xy + 2y^2", "5y^2-3xy+2y^2")]
        [InlineData("x - 2", "x-2")]
        [InlineData("1", "1")]
        [InlineData("x - (x^2 - xy - 2)", "x-(x^2-xy-2)")]
        [InlineData("x - (x^0 - (0 - x + y - y^0))", "x-(1-(0-x+y-1))")]
        public void ParseExpression__ShouldBeOk(string input, string expected)
        {
            var parseResult = Expression.TryParse(input, out var expression);

            Assert.True(parseResult.IsSuccessfull, parseResult.GetErrorsString());
            Assert.Equal(expected, expression.ToString());
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
