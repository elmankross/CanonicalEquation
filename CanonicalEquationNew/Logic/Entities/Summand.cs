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

        private readonly Guid _id;
        private readonly StringBuilder _buffer;
        private const string MULTIPLIER_GROUP = "multiplier";
        private const string VARIABLE_GROUP = "variable";
        private static readonly Regex Pattern = new Regex($"^(?<{MULTIPLIER_GROUP}>\\-*\\d*\\.*\\,*\\d*)(?<{VARIABLE_GROUP}>(\\w\\^\\-*\\d|\\w))*$", RegexOptions.Compiled);

        private Summand()
        {
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

            // COMPLEX SUMMAND
            if (input.Contains(Symbols.OPEN_BRACKET))
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

                simpleSummand.Summands.Add(complexSummand);
                summand = simpleSummand;
            }
            // SIMPLE SUMMAND
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
                if (!string.IsNullOrEmpty(multiplierStr))
                {
                    if (multiplierStr == "-")
                    {
                        multiplierStr = "-1";
                    }

                    if (!float.TryParse(multiplierStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var multiplier))
                    {
                        result.AddError("Invalid multiplier summand format", multiplierStr);
                        return result;
                    }

                    summand.Multiplier = multiplier;
                }
                else
                {
                    summand.Multiplier = 1f;
                }
            }
            // SUMMANDS
            else if (input.Contains(Symbols.PLUS) || input.Contains(Symbols.MINUS))
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
    }
}
