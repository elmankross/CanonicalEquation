using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestApp.Math
{
    public class AlgebraicSummand : IAlgebraicSummand
    {
        public const string PATTERN = "^(?<multiplier>\\-?\\d*)((?<variable>\\w?)|\\^(?<power>\\d))+$";

        public float Multiplier { get; private set; }
        public HashSet<AlgebraicVariable> Variables { get; private set; }

        private AlgebraicSummand() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IAlgebraicSummand other)
        {
            throw new NotImplementedException();
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
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Multiplier.Equals(1) || Multiplier.Equals(-1))
            {
                sb.Append(float.IsNegative(Multiplier) ? Symbols.Minus : Symbols.Plus);
            }
            else
            {
                if (!float.IsNegative(Multiplier))
                {
                    sb.Append(Symbols.Plus);
                }
                sb.Append(Multiplier);
            }

            foreach (var variable in Variables)
            {
                sb.Append(variable);
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
            if (string.IsNullOrEmpty(input))
            {
                input = "1";
            }

            if (input.Equals("-"))
            {
                input = "-1";
            }

            return float.TryParse(input, out multiplier);
        }
    }
}
