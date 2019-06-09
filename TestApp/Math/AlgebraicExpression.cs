using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp.Math
{
    public class AlgebraicExpression
    {
        public HashSet<IAlgebraicSummand> Summands { get; private set; }

        private AlgebraicExpression() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expression"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out AlgebraicExpression expression, out string error)
        {
            expression = null;
            error = null;

            if (!CheckContainsValidSymbols(input))
            {
                var sb = new StringBuilder();
                sb.Append("Expression contains invalid symbols. Allowed only letters, digits and special symbols: ");
                sb.Append(Symbols.AllowedSymbols);
                error = sb.ToString();
                return false;
            }

            if (!AlgebraicSummandGroup.TryParse(input, out var summands, out error))
            {
                return false;
            }

            expression = new AlgebraicExpression
            {
                Summands = summands.Summands
            };

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var summand in Summands)
            {
                sb.Append(summand);
            }
            var res = sb.ToString();
            if (res.StartsWith(Symbols.Plus))
            {
                res = res.Remove(0, 1);
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool CheckContainsValidSymbols(string input)
        {
            return !string.IsNullOrEmpty(input)
                && input.All(@char => char.IsLetterOrDigit(@char) || Symbols.AllowedSymbols.Contains(@char));
        }
    }
}