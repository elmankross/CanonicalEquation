using System.Text;

namespace Logic.Entities
{
    public class Equation
    {
        public Expression LeftExpression { get; private set; }
        public Expression RightExpression { get; private set; }

        private readonly StringBuilder _buffer;

        private Equation()
        {
            _buffer = new StringBuilder();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="equation"></param>
        /// <returns></returns>
        public static CallResult TryParse(string input, out Equation equation)
        {
            equation = new Equation();
            var result = new CallResult();

            if (string.IsNullOrEmpty(input))
            {
                result.AddError("Input is empty");
                return result;
            }

            var parts = input.Split('=');

            if (parts.Length != 2)
            {
                result.AddError("Equation should contains one '=' sign", input);
                return result;
            }

            var leftParseResult = Expression.TryParse(parts[0], out var leftExpression);
            var rightParseResult = Expression.TryParse(parts[1], out var rightExpression);

            if (!leftParseResult.IsSuccessfull || !rightParseResult.IsSuccessfull)
            {
                leftParseResult.CopyErrorsTo(result);
                rightParseResult.CopyErrorsTo(result);
                return result;
            }

            equation.LeftExpression = leftExpression;
            equation.RightExpression = rightExpression;

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Equation GetCanonicalForm()
        {
            return new Equation();
        }


        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return _buffer.Clear()
                          .Append(LeftExpression)
                          .Append(Symbols.EQUAL)
                          .Append(RightExpression)
                          .ToString();
        }

        #endregion
    }
}
