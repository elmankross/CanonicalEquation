using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Logic.Entities
{
    public class Summand : IEquatable<Summand>, ICloneable
    {
        public float Multiplier { get; private set; }
        public HashSet<Variable> Variables { get; private set; }
        public HashSet<Summand> Summands { get; }

        public float Priority
        {
            get
            {
                var priority = 0f;
                if (Variables.Count > 0)
                {
                    priority += (float)Variables.Sum(v => Math.Pow(v.Power, 2)) / Variables.Count;
                    priority += Variables.Sum(v => 'z' - v.Name);
                }

                return priority;
            }
        }

        // Helps to avoid double brackets during parsing subsummands. Like -((xyz-xyz))
        private bool _isSubsummands;
        private readonly Guid _id;
        private readonly StringBuilder _buffer;
        private const string MULTIPLIER_GROUP = "multiplier";
        private const string VARIABLE_GROUP = "variable";
        private static readonly Regex Pattern = new Regex($"^(?<{MULTIPLIER_GROUP}>[\\-\\+]*\\d*\\.*\\,*\\d*)(?<{VARIABLE_GROUP}>(\\w\\^\\-*\\d|\\w))*$", RegexOptions.Compiled);

        private Summand()
        {
            _isSubsummands = false;
            _id = Guid.NewGuid();
            _buffer = new StringBuilder();
            Variables = new HashSet<Variable>();
            Summands = new HashSet<Summand>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CallResult TryParse(string input, out Summand summand)
        {
            summand = new Summand();
            var result = new CallResult();

            if (string.IsNullOrEmpty(input))
            {
                result.AddError("Summand is empty");
                return result;
            }

            // 1. Complex summand
            if (CheckComplexSummand(input))
            {
                var beginBlock = input.IndexOf(Symbols.OPEN_BRACKET) + 1;
                var endBlock = input.LastIndexOf(Symbols.CLOSE_BRACKET);

                // Base part of complex summand. All outside brackets
                var parseResultSimple = TryParse(beginBlock == 1 ? "1" : input.Substring(0, beginBlock - 1), out var simpleSummand);
                // Internal part of complex summand. All inside brackets
                var parseResultComplex = TryParse(input.Substring(beginBlock, endBlock - beginBlock), out var complexSummand);

                if (!parseResultSimple.IsSuccessfull || !parseResultComplex.IsSuccessfull)
                {
                    parseResultSimple.CopyErrorsTo(result);
                    parseResultComplex.CopyErrorsTo(result);
                    return result;
                }

                if (complexSummand._isSubsummands)
                {
                    foreach (var s in complexSummand.Summands)
                    {
                        simpleSummand.Summands.Add(s);
                    }
                }
                else
                {
                    simpleSummand.Summands.Add(complexSummand);
                }

                summand = simpleSummand;
            }
            // 2. Subsummands
            else if (CheckSubsummands(input))
            {
                var summandsEnumerator = new SummandsEnumerator(input);
                while (summandsEnumerator.MoveNext())
                {
                    var parseResult = TryParse(summandsEnumerator.Current, out var simpleSummand);
                    if (!parseResult.IsSuccessfull)
                    {
                        result.AddError("Invalid summand input format", summandsEnumerator.Current);
                        return result;
                    }

                    summand.Summands.Add(simpleSummand);
                }

                // HACK: helps to avoid double brackets outside
                summand._isSubsummands = true;
            }
            // 3. Simple summand
            else if (Pattern.IsMatch(input))
            {
                var regex = Pattern.Match(input);

                foreach (Capture variableStr in regex.Groups[VARIABLE_GROUP].Captures)
                {
                    var parseResult = Variable.TryParse(variableStr.Value, out var variable);
                    if (!parseResult.IsSuccessfull)
                    {
                        result.AddError("Invalid variable input format", variableStr.Value);
                        return result;
                    }

                    if (summand.Variables.Any(v => v.Name == variable.Name))
                    {
                        var existsVariable = summand.Variables.Single(v => v.Name == variable.Name);
                        //summand.Variables.TryGetValue(variable, out var existsVariable);
                        summand.Variables.Remove(existsVariable);
                        summand.Variables.Add(existsVariable * variable);
                    }
                    else
                    {
                        summand.Variables.Add(variable);
                    }
                }

                var multiplierStr = regex.Groups[MULTIPLIER_GROUP].Value;
                var multiplierParseResult = TryParseMultiplier(multiplierStr, out var multiplier);
                if (!multiplierParseResult.IsSuccessfull)
                {
                    multiplierParseResult.CopyErrorsTo(result);
                    return result;
                }

                summand.Multiplier = multiplier;
            }
            else
            {
                result.AddError("Invalid summand format", input);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="summands"></param>
        /// <returns></returns>
        public bool TrySimplify(out HashSet<Summand> summands)
        {
            summands = null;
            if (Summands.Count > 0)
            {
                summands = new HashSet<Summand>(Summands.Count);
                foreach (var summand in Summands)
                {
                    if (summand.TrySimplify(out var subsummands))
                    {
                        foreach (var subsummand in subsummands)
                        {
                            summands.Add(this * subsummand);
                        }
                    }
                    else
                    {
                        summands.Add(this * summand);
                    }
                }

                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanAdd(Summand other)
        {
            return Variables.All(v => v.Power == 0) && other.Variables.All(v => v.Power == 0)
                || Variables.Count == 0 && other.Variables.Count == 0
                || Multiplier.Equals(0) && other.Variables.Count == 0
                || other.Multiplier.Equals(0) && Variables.Count == 0
                || Variables.SetEquals(other.Variables);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Summand other)
        {
            return _id == other._id;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Summand operator +(Summand left, Summand right)
        {
            var result = new Summand();
            if (left.Variables.SetEquals(right.Variables))
            {
                result.Variables = left.Variables;
            }

            result.Multiplier = left.Multiplier + right.Multiplier;

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Summand operator *(Summand left, Summand right)
        {
            var resultSummand = new Summand
            {
                Multiplier = left.Multiplier * right.Multiplier,
            };

            var variables = left.Variables.ToList();
            variables.AddRange(right.Variables);
            resultSummand.Variables = variables.GroupBy(v => v.Name)
                                               .Aggregate(new List<Variable>(), (set, grouping) =>
                                                {
                                                    var result = grouping.Aggregate((current, next) => current * next);
                                                    set.Add(result);
                                                    return set;
                                                }).ToHashSet();

            return resultSummand;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Invert()
        {
            Multiplier *= -1;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Sort()
        {
            var newSet = new HashSet<Summand>();
            foreach (var summand in Summands.OrderByDescending(s => s.Priority))
            {
                summand.Sort();
                newSet.Add(summand);
            }

            Summands.Clear();
            foreach (var s in newSet)
            {
                Summands.Add(s);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            _buffer.Clear();

            if (Math.Abs(Multiplier).Equals(0))
            {
                return _buffer.Append(Multiplier).ToString();
            }

            _buffer.Append(float.IsNegative(Multiplier) ? Symbols.MINUS : Symbols.PLUS);

            if (Math.Abs(Multiplier).Equals(1))
            {
                if (Variables.Count(v => v.Power != 0) == 0 && Summands.Count == 0)
                {
                    _buffer.Append(Math.Abs(Multiplier));
                }
            }
            else
            {
                _buffer.Append(Math.Abs(Multiplier));
            }

            foreach (var variable in Variables.OrderBy(v => v.Name))
            {
                _buffer.Append(variable);
            }

            if (Summands.Count > 0)
            {
                _buffer.Append(Symbols.OPEN_BRACKET);
                var summandEnumerator = new TrimerEnumerator<Summand>(Summands);
                while (summandEnumerator.MoveNext())
                {
                    _buffer.Append(summandEnumerator.Current);
                }
                _buffer.Append(Symbols.CLOSE_BRACKET);
            }

            return _buffer.ToString();
        }


        /// <inheritdoc />
        public object Clone()
        {
            var clone = new Summand
            {
                Multiplier = Multiplier,
                Variables = new HashSet<Variable>(Variables),
                _isSubsummands = _isSubsummands
            };

            foreach (var summand in Summands.Select(s => s.Clone()))
            {
                clone.Summands.Add(summand as Summand);
            }

            return clone;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        private static CallResult TryParseMultiplier(string input, out float multiplier)
        {
            var result = new CallResult();
            if (!string.IsNullOrEmpty(input))
            {
                input = input == "-" ? "-1" : input;
                input = input == "+" ? "+1" : input;

                if (!float.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out multiplier))
                {
                    result.AddError("Invalid multiplier summand format", input);
                }
            }
            else
            {
                multiplier = 1f;
            }

            return result;
        }


        /// <summary>
        /// Check if the input has format like: ax^k+ax^k
        /// </summary>
        /// <returns></returns>
        private static bool CheckSubsummands(string input)
        {
            var startSearchIndex = input.StartsWith(Symbols.MINUS) || input.StartsWith(Symbols.PLUS)
                ? 1
                : 0;

            for (; startSearchIndex < input.Length; startSearchIndex++)
            {
                var plusIndex = input.IndexOf(Symbols.PLUS, startSearchIndex);
                var minusIndex = input.IndexOf(Symbols.MINUS, startSearchIndex);
                var nstSignIndex = 0;

                if (plusIndex == -1 && minusIndex == -1)
                {
                    return false;
                }

                if (plusIndex == -1)
                {
                    nstSignIndex = minusIndex;
                }
                else if (minusIndex == -1)
                {
                    nstSignIndex = plusIndex;
                }
                else
                {
                    nstSignIndex = Math.Min(plusIndex, minusIndex);
                }

                var prevSignSymbolIsPower = nstSignIndex > 0 && input[nstSignIndex - 1] == Symbols.POWER;

                if (input.Contains(Symbols.OPEN_BRACKET) && input.Contains(Symbols.CLOSE_BRACKET))
                {
                    var nstOpenBracketIndex = input.IndexOf(Symbols.OPEN_BRACKET, startSearchIndex);
                    var nstCloseBracketIndex = input.IndexOf(Symbols.CLOSE_BRACKET, startSearchIndex);

                    if (nstOpenBracketIndex != -1 && nstCloseBracketIndex != -1)
                    {
                        var bracketsRange = Enumerable.Range(nstOpenBracketIndex,
                            nstCloseBracketIndex - nstOpenBracketIndex);
                        if (nstSignIndex != -1)
                        {
                            return !bracketsRange.Contains(nstSignIndex);
                        }

                        if (startSearchIndex < input.Length)
                        {
                            continue;
                        }

                        return false;
                    }
                }
                else
                {
                    return nstSignIndex != -1 && !prevSignSymbolIsPower;
                }
            }
            return false;
        }


        /// <summary>
        /// Check if the input has format like ax^k(ax^k+ax^k)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool CheckComplexSummand(string input)
        {
            var beginBlockIndex = input.IndexOf(Symbols.OPEN_BRACKET);
            var endBlockIndex = input.LastIndexOf(Symbols.CLOSE_BRACKET);

            if (beginBlockIndex == -1)
            {
                return false;
            }

            if (endBlockIndex != input.Length - 1)
            {
                return false;
            }

            return Pattern.IsMatch(input.Substring(0, beginBlockIndex));
        }
    }
}
