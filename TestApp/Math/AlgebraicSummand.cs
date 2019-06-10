using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestApp.Math
{
    public class AlgebraicSummand : IEquatable<AlgebraicSummand>
    {
        private const string PATTERN = "^(?<multiplier>[\\-\\+]?\\d*)((?<variable>\\w?)|\\^(?<power>\\d))+$";

        public float Multiplier { get; private set; }
        public HashSet<AlgebraicVariable> Variables { get; private set; }
        private readonly HashSet<AlgebraicSummand> _summands;

        private AlgebraicSummand()
        {
            _summands = new HashSet<AlgebraicSummand>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AlgebraicSummand other)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public void AddSubsummand(AlgebraicSummand summand)
        {
            _summands.Add(summand);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="summand"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out AlgebraicSummand summand, out string error)
        {
            summand = null;
            error = null;

            if (!IsValid(input))
            {
                error = "Invalid summand format";
                return false;
            }

            var regex = Regex.Match(input, PATTERN);
            TryParseMultiplier(regex.Groups["multiplier"].Value, out var multiplier);
            summand = new AlgebraicSummand
            {
                Multiplier = multiplier,
                Variables = regex.Groups["variable"].Captures.Select((c, i) =>
                {
                    if (c.Length > 0)
                    {
                        var power = 1;
                        var powerCaptures = regex.Groups["power"].Captures;
                        if (powerCaptures.Count > i)
                        {
                            power = int.Parse(powerCaptures[i].Value);
                        }
                        return new AlgebraicVariable(c.Value[0], power);
                    }
                    return null;
                }).Where(r => r != null).ToHashSet()
            };

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsValid(string input)
        {
            return Regex.IsMatch(input, PATTERN);
        }


        /// <summary>
        /// Split expression to outer part and inter part
        /// For example: -(x+2) will be:
        ///     - outer = -1
        ///     - inter = x+2
        /// </summary>
        /// <returns></returns>
        public static (string outer, string inter) Unwrap(string input)
        {
            var openBracketIndex = input.IndexOf(Symbols.OpenBracket);
            if (openBracketIndex == -1)
            {
                // TODO: 
                return (outer: string.Empty, inter: string.Empty);
            }

            var outer = string.Empty;
            if (openBracketIndex > 0)
            {
                outer = input.Substring(0, openBracketIndex);
                input = input.Remove(0, openBracketIndex + 1);

                if (outer.Equals("-"))
                {
                    outer = "-1";
                }
            }
            else
            {
                input = input.Remove(openBracketIndex, 1);
            }

            var closeBracketIndex = input.LastIndexOf(Symbols.CloseBracket);
            if (closeBracketIndex == -1)
            {
                // TODO: 
                return (outer: string.Empty, inter: string.Empty);
            }

            return (
                outer: outer,
                inter: input.Remove(closeBracketIndex, 1)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Multiplier.Equals(1) || Multiplier.Equals(-1))
            {
                sb.Append(float.IsNegative(Multiplier) ? Symbols.Minus : Symbols.Plus);
                if (Variables.Count == 0 && _summands.Count == 0)
                {
                    sb.Append(Multiplier);
                }
            }
            else
            {
                if (!(Variables.Count > 0 && Multiplier.Equals(0)))
                {
                    sb.Append(!float.IsNegative(Multiplier) ? Symbols.Plus : (char?)null);
                    sb.Append(Multiplier);
                }
            }

            foreach (var variable in Variables)
            {
                sb.Append(variable);
            }

            if (_summands.Count > 0)
            {
                sb.Append(Symbols.OpenBracket);
                var ssb = new StringBuilder();
                foreach (var summand in _summands)
                {
                    ssb.Append(summand);
                }

                var summandsStr = ssb.ToString();
                if (summandsStr.StartsWith(Symbols.Plus))
                {
                    summandsStr = summandsStr.Remove(0, 1);
                }

                sb.Append(summandsStr);
                sb.Append(Symbols.CloseBracket);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        private static bool TryParseMultiplier(string input, out float multiplier)
        {
            if (string.IsNullOrEmpty(input) || input == Symbols.Plus.ToString())
            {
                input = "1";
            }

            if (input == Symbols.Minus.ToString())
            {
                input = "-1";
            }

            return float.TryParse(input, out multiplier);
        }
    }
}
