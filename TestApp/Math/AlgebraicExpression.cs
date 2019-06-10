using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp.Math
{
    public class AlgebraicExpression
    {
        public HashSet<AlgebraicSummand> Summands { get; }

        private AlgebraicExpression()
        {
            Summands = new HashSet<AlgebraicSummand>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expression"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out AlgebraicExpression expression, out string error)
        {
            expression = new AlgebraicExpression();
            error = null;

            if (!CheckContainsValidSymbols(input))
            {
                var sb = new StringBuilder();
                sb.Append("Expression contains invalid symbols. Allowed only letters, digits and special symbols: ");
                sb.Append(Symbols.AllowedSymbols);
                error = sb.ToString();
                return false;
            }

            if (TryParseSummand(input, out var summands, out error))
            {
                foreach (var summand in summands)
                {
                    expression.Summands.Add(summand);
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool TryParseSummand(string input, out HashSet<AlgebraicSummand> summands, out string error)
        {
            summands = new HashSet<AlgebraicSummand>();
            error = null;

            var enumerator = new ExpressionEnumerator(input);
            while (enumerator.MoveNext())
            {
                if (AlgebraicSummand.TryParse(enumerator.Current, out var simpleSummand, out error))
                {
                    summands.Add(simpleSummand);
                }
                else
                {
                    var (outer, inter) = AlgebraicSummand.Unwrap(enumerator.Current);
                    if (TryParseSummand(outer, out var outerSummands, out error))
                    {
                        if (TryParseSummand(inter, out var interSummands, out error))
                        {
                            if (outerSummands.Count > 1)
                            {
                                // TODO: SHOULD BE ONLY ONE!
                            }
                            else
                            {
                                var outerSummand = outerSummands.First();
                                foreach (var interSummand in interSummands)
                                {
                                    outerSummand.AddSubsummand(interSummand);
                                }
                                summands.Add(outerSummand);
                            }
                        }
                    }
                }
            }

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