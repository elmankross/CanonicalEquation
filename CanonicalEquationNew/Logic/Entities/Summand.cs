using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Logic.Entities
{
    public class Summand : IEquatable<Summand>
    {
        public float Multiplier { get; private set; }
        public HashSet<Variable> Variables { get; }
        public HashSet<Summand> Summands { get; }

        // Help to avoid double brackets during parsing subsummands. Like -((xyz-xyz))
        private bool _isSubsummands;
        private readonly Guid _id;
        private readonly StringBuilder _buffer;
        private const string MULTIPLIER_GROUP = "multiplier";
        private const string VARIABLE_GROUP = "variable";
        private static readonly Regex Pattern = new Regex($"^(?<{MULTIPLIER_GROUP}>\\-*\\d*\\.*\\,*\\d*)(?<{VARIABLE_GROUP}>(\\w\\^\\-*\\d|\\w))*$", RegexOptions.Compiled);

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

            // 1. Complex summand
            if (CheckComplexSummand(input))
            {
                var startBlock = input.IndexOf(Symbols.OPEN_BRACKET) + 1;
                var endBlock = input.LastIndexOf(Symbols.CLOSE_BRACKET);

                // Base part of complex summand. All outside brackets
                var parseResultSimple = TryParse(input.Substring(0, startBlock - 1), out var simpleSummand);
                // Internal part of complex summand. All inside brackets
                var parseResultComplex = TryParse(input.Substring(startBlock, endBlock - startBlock), out var complexSummand);

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

                    if (summand.Variables.Contains(variable))
                    {
                        summand.Variables.TryGetValue(variable, out var existsVariable);
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
                result.AddError("Invalid summand input format", input);
            }

            return result;
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
        /// <returns></returns>
        public override string ToString()
        {
            _buffer.Clear();

            if (Math.Abs(Multiplier).Equals(0))
            {
                return _buffer.ToString();
            }

            _buffer.Append(float.IsNegative(Multiplier) ? Symbols.MINUS : Symbols.PLUS);

            if (Math.Abs(Multiplier).Equals(1))
            {
                if (Variables.Count == 0 && Summands.Count == 0)
                {
                    _buffer.Append(Math.Abs(Multiplier));
                }
            }
            else
            {
                if (!Math.Abs(Multiplier).Equals(0))
                {
                    _buffer.Append(Math.Abs(Multiplier));
                }
            }

            foreach (var variable in Variables.OrderBy(v => v.Name))
            {
                _buffer.Append(variable);
            }

            if (Summands.Count > 0)
            {
                _buffer.Append(Symbols.OPEN_BRACKET);
                var index = 0;
                foreach (var summand in Summands)
                {
                    var smdString = summand.ToString();
                    if (index == 0)
                    {
                        if (smdString.StartsWith(Symbols.PLUS) || smdString.StartsWith(Symbols.MINUS))
                        {
                            smdString = smdString.Remove(0, 1);
                        }
                    }
                    _buffer.Append(smdString);
                    index++;
                }
                _buffer.Append(Symbols.CLOSE_BRACKET);
            }

            return _buffer.ToString();
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

            for (; startSearchIndex < input.Length ; startSearchIndex++)
            {
                var nstSignIndex = input.IndexOf(Symbols.PLUS, startSearchIndex) != -1
                                   ? input.IndexOf(Symbols.PLUS, startSearchIndex)
                                   : input.IndexOf(Symbols.MINUS, startSearchIndex);
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
