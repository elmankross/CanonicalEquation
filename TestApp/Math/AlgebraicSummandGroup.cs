using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestApp.Math
{
    public class AlgebraicSummandGroup : IAlgebraicSummand
    {
        public float Multiplier { get; private set; }
        public HashSet<IAlgebraicSummand> Summands { get; }

        private AlgebraicSummandGroup()
        {
            Summands = new HashSet<IAlgebraicSummand>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="summand"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out AlgebraicSummandGroup summand, out string error)
        {
            error = null;
            summand = new AlgebraicSummandGroup();

            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            var sb = new StringBuilder(input.Length);

            for (var i = 0; i < input.Length; i++)
            {
                var currentSymbol = input[i];
                if (currentSymbol == Symbols.OpenBracket)
                {
                    var closeBracketIndex = input.IndexOf(Symbols.CloseBracket);
                    sb.Append(input.Substring(i + 1, closeBracketIndex - i - 1));
                    i = closeBracketIndex;
                }
                else if (Symbols.AllowedMathOperators.Contains(currentSymbol) && i != 0)
                {
                    var block = sb.ToString();
                    if (TryParseSummand(block, out var smd1, out error))
                    {
                        summand.Summands.Add(smd1);
                    }
                    else
                    {
                        return false;
                    }

                    sb.Clear().Append(currentSymbol);
                }
                else
                {
                    sb.Append(currentSymbol);
                }
            }

            if (TryParseSummand(sb.ToString(), out var smd2, out error))
            {
                summand.Summands.Add(smd2);
                sb.Clear();
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IAlgebraicSummand other)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(float.IsNegative(Multiplier) ? Symbols.Empty : Symbols.Plus);
            if (Multiplier.Equals(1))
            {
                sb.Append(Multiplier);
            }
            sb.Append(Symbols.OpenBracket);
            foreach (var summand in Summands)
            {
                sb.Append(summand);
            }
            sb.Append(Symbols.CloseBracket);
            return sb.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool TryParseSummand(string block, out IAlgebraicSummand summand, out string error)
        {
            if (IsSimple(block))
            {
                if (AlgebraicSummand.TryParse(block, out var algebraicSummand, out error))
                {
                    summand = algebraicSummand;
                    return true;
                }
            }
            else if (TryParse(block, out var algebraicSummandGroup, out error))
            {
                summand = algebraicSummandGroup;
                return true;
            }

            summand = null;
            return false;
        }


        /// <summary>
        /// Проверка на то, что переданный блок не может быть разбит на слагаемые
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static bool IsSimple(string block)
        {
            return Regex.IsMatch(block, AlgebraicSummand.PATTERN);
        }
    }
}
