using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Entities
{
    public class Equation
    {
        public Expression LeftExpression { get; private set; }
        public Expression RightExpression { get; private set; }

        private readonly StringBuilder _buffer;

        private Equation()
        {
            _buffer = new StringBuilder();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="equation"></param>
        /// <returns></returns>
        public static CallResult TryParse(string input, out Equation equation)
        {
            equation = new Equation();
            var result = new CallResult();

            if (string.IsNullOrEmpty(input))
            {
                result.AddError("Input is empty");
                return result;
            }

            var parts = input.Split('=');

            if (parts.Length != 2)
            {
                result.AddError("Equation should contains one '=' sign", input);
                return result;
            }

            var leftParseResult = Expression.TryParse(parts[0], out var leftExpression);
            var rightParseResult = Expression.TryParse(parts[1], out var rightExpression);

            if (!leftParseResult.IsSuccessfull || !rightParseResult.IsSuccessfull)
            {
                leftParseResult.CopyErrorsTo(result);
                rightParseResult.CopyErrorsTo(result);
                return result;
            }

            equation.LeftExpression = leftExpression;
            equation.RightExpression = rightExpression;

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Equation GetCanonicalForm()
        {
            var result = new Equation
            {
                LeftExpression = LeftExpression - RightExpression
            };

            Expression.TryParse("0", out var rightExpression);
            result.RightExpression = rightExpression;

            var resultSummands = new HashSet<Summand>();
            foreach (var summand in result.LeftExpression.Summands)
            {
                if (summand.TrySimplify(out var simplifiedSummands))
                {
                    foreach (var simplifiedSummand in simplifiedSummands)
                    {
                        resultSummands.Add(simplifiedSummand);
                    }
                }
                else
                {
                    resultSummands.Add(summand);
                }
            }

            while (true)
            {
                var used = new HashSet<Summand>();
                var subtrated = new HashSet<Summand>();
                var breakerCount = 0;
                var breakerLimit = (int)Math.Pow(resultSummands.Count, 2);

                foreach (var o in resultSummands)
                {
                    foreach (var i in resultSummands)
                    {
                        if (o.Equals(i))
                        {
                            breakerCount++;
                            continue;
                        }

                        if (o.CanAdd(i))
                        {
                            if (!used.Contains(o) && !used.Contains(i))
                            {
                                subtrated.Add(o + i);
                                used.Add(o);
                                used.Add(i);
                                break;
                            }
                        }

                        breakerCount++;
                    }
                }

                foreach (var u in used)
                {
                    resultSummands.Remove(u);
                }

                foreach (var s in subtrated)
                {
                    resultSummands.Add(s);
                }

                used.Clear();
                subtrated.Clear();

                if (breakerCount == breakerLimit)
                {
                    result.LeftExpression.Summands.Clear();
                    foreach (var r in resultSummands.OrderByDescending(s => s.Priority))
                    {
                        r.Sort();
                        result.LeftExpression.Summands.Add(r);
                    }
                    break;
                }
            }

            return result;
        }


        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return _buffer.Clear()
                          .Append(LeftExpression)
                          .Append(Symbols.EQUAL)
                          .Append(RightExpression)
                          .ToString();
        }

        #endregion
    }
}
