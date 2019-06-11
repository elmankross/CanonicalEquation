using System;

namespace TestApp.Math
{
    public class AlgebraicEquation
    {
        private AlgebraicEquation() { }

        public AlgebraicExpression LeftExpression { get; private set; }
        public AlgebraicExpression RightExpression { get; private set; }


        /// <summary>
        /// Разобрать уравнение из входной строки
        /// </summary>
        /// <param name="input"></param>
        /// <param name="equation"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out AlgebraicEquation equation, out string error)
        {
            equation = null;
            error = null;

            if (string.IsNullOrEmpty(input))
            {
                error = "No input data";
                return false;
            }

            input = input.Replace(" ", null);
            var equationParts = input.Split(Symbols.Equal, StringSplitOptions.RemoveEmptyEntries);

            if (equationParts.Length < 2)
            {
                error = $"Input string is not equation: input must contains '{Symbols.Equal}' sign";
                return false;
            }

            if (equationParts.Length > 2)
            {
                error = $"Input string has invalid equation format: '{Symbols.Equal}' sign must be only one";
                return false;
            }

            if (!AlgebraicExpression.TryParse(equationParts[0], out var leftExpression, out error))
            {
                return false;
            }

            if (!AlgebraicExpression.TryParse(equationParts[1], out var rightExpression, out error))
            {
                return false;
            }

            equation = new AlgebraicEquation
            {
                LeftExpression = leftExpression,
                RightExpression = rightExpression
            };

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public void ToCanonicalForm()
        {
            LeftExpression = LeftExpression - RightExpression;
            if (AlgebraicExpression.TryParse("0", out var rightExpression, out _))
            {
                RightExpression = rightExpression;
            }
            LeftExpression.Normilize();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{LeftExpression}{Symbols.Equal}{RightExpression}";
        }
    }
}
