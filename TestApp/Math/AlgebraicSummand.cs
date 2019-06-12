using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestApp.Math
{
    public class AlgebraicSummand : IEquatable<AlgebraicSummand>
    {
        private readonly Guid _id;
        private static readonly Regex Pattern = new Regex("^(?<multiplier>[\\-\\+]?[\\.\\d]*)((?<variable>\\w?)|\\^(?<power>\\d))+$", RegexOptions.Compiled);
        public HashSet<AlgebraicSummand> Summands { get; private set; }

        public float Multiplier { get; private set; }
        public HashSet<AlgebraicVariable> Variables { get; private set; }

        public int Priority
        {
            get
            {
                if (Variables.Any())
                {
                    if (Variables.All(v => v.Power == 1))
                    {
                        return Variables.Count;
                    }

                    // Var names unicode range: 65-122
                    return Variables.Max(v => v.Power) + Variables.Max(v => 122 - v.Name);
                }
                return 0;
            }
        }



        private AlgebraicSummand()
        {
            Summands = new HashSet<AlgebraicSummand>();
            _id = Guid.NewGuid();
        }

        #region Equality members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AlgebraicSummand other)
        {
            return _id == other._id;
        }

        #endregion


        #region Overrides of Object

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion


        /// <summary>
        /// Indicates both summands has the same rank
        /// </summary>
        /// <returns></returns>
        public bool TheSame(AlgebraicSummand other)
        {
            return Variables.SetEquals(other.Variables)
                && Summands.SetEquals(other.Summands);
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

            var regex = Pattern.Match(input);
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
            return Pattern.IsMatch(input);
        }


        /// <summary>
        /// Check if current summand contains subsummands
        /// </summary>
        /// <returns></returns>
        public bool TryNormilize()
        {
            if (Summands.Any())
            {
                Summands = Summands.Select(s =>
                {
                    if (s.TryNormilize())
                    {
                        s.Multiplier = 0f;
                        s.Variables.Clear();
                    }
                    s *= this;
                    return s;
                }).ToHashSet();
                Multiplier = 0f;
                Variables.Clear();
                return true;
            }

            return false;
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

            if (System.Math.Abs(Multiplier).Equals(1))
            {
                sb.Append(float.IsNegative(Multiplier) ? Symbols.Minus : Symbols.Plus);
                if (Variables.Count == 0 && Summands.Count == 0)
                {
                    sb.Append(System.Math.Abs(Multiplier));
                }
            }
            else
            {
                if (Variables.Count > 0 && Multiplier.Equals(0))
                {
                    return sb.ToString();
                }

                sb.Append(!float.IsNegative(Multiplier) ? Symbols.Plus : (char?)null);
                sb.Append(Multiplier);
            }

            foreach (var variable in Variables)
            {
                sb.Append(variable);
            }

            if (Summands.Count > 0)
            {
                sb.Append(Symbols.OpenBracket);
                var ssb = new StringBuilder();
                foreach (var summand in Summands)
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

            var result = sb.ToString();
            return result.Replace(',', Symbols.AllowedFloatPoints[0]);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="smd1"></param>
        /// <param name="smd2"></param>
        /// <returns></returns>
        public static AlgebraicSummand operator -(AlgebraicSummand smd1, AlgebraicSummand smd2)
        {
            smd1.Variables.UnionWith(smd2.Variables);
            return new AlgebraicSummand
            {
                Multiplier = smd1.Multiplier - smd2.Multiplier,
                Variables = smd1.Variables
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="smd1"></param>
        /// <param name="smd2"></param>
        /// <returns></returns>
        public static AlgebraicSummand operator +(AlgebraicSummand smd1, AlgebraicSummand smd2)
        {
            smd1.Variables.UnionWith(smd2.Variables);
            return new AlgebraicSummand
            {
                Multiplier = smd1.Multiplier + smd2.Multiplier,
                Variables = smd1.Variables
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="smd1"></param>
        /// <param name="smd2"></param>
        /// <returns></returns>
        public static AlgebraicSummand operator *(AlgebraicSummand smd1, AlgebraicSummand smd2)
        {
            var resultVariables = new HashSet<AlgebraicVariable>();

            foreach (var smd1Variable in smd1.Variables)
            {
                foreach (var smd2Variable in smd2.Variables)
                {
                    if (smd1Variable.Equals(smd2Variable))
                    {
                        resultVariables.Add(new AlgebraicVariable(smd1Variable.Name, smd1Variable.Power + 1));
                    }
                    else
                    {
                        resultVariables.Add(smd2Variable);
                    }
                }
                resultVariables.Add(smd1Variable);
            }

            return new AlgebraicSummand
            {
                Multiplier = smd1.Multiplier * smd2.Multiplier,
                Variables = resultVariables
            };
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

            return float.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out multiplier);
        }
    }
}
