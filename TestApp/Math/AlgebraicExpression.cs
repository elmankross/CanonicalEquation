using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp.Math
{
    public class AlgebraicExpression
    {
        public HashSet<AlgebraicSummand> Summands { get; private set; }

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

            if (TryParseSummands(input, out var summands, out error))
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
        private static bool TryParseSummands(string input, out HashSet<AlgebraicSummand> summands, out string error)
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
                    if (TryParseSummands(outer, out var outerSummands, out error))
                    {
                        if (TryParseSummands(inter, out var interSummands, out error))
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
                                    outerSummand.Summands.Add(interSummand);
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
            return res.Replace("+0", string.Empty).Replace("-0", string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static AlgebraicExpression operator -(AlgebraicExpression exp1, AlgebraicExpression exp2)
        {
            var result = new AlgebraicExpression();

            foreach (var resultSummand in MathUnion(exp1.Summands, exp2.Summands))
            {
                result.Summands.Add(resultSummand);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static HashSet<AlgebraicSummand> MathUnion(HashSet<AlgebraicSummand> left, HashSet<AlgebraicSummand> right)
        {
            var usedRight = new HashSet<AlgebraicSummand>();
            var usedLeft = new HashSet<AlgebraicSummand>();
            var resTemp = new HashSet<AlgebraicSummand>();
            var res = new HashSet<AlgebraicSummand>();

            foreach (var l in left)
            {
                foreach (var r in right)
                {
                    if (l.TheSame(r))
                    {
                        resTemp.Remove(l);
                        resTemp.Add(l - r);

                        usedLeft.Add(l);
                        usedRight.Add(r);

                        break;
                    }
                }

                if (!usedLeft.Contains(l))
                {
                    resTemp.Add(l);
                }
            }

            foreach (var r in right)
            {
                if (!usedRight.Contains(r))
                {
                    AlgebraicSummand.TryParse("0", out var zeroSummand, out _);
                    resTemp.Add(zeroSummand - r);
                }
            }

            usedLeft.Clear();
            usedRight.Clear();

            foreach (var temp1 in resTemp)
            {
                foreach (var temp2 in resTemp)
                {
                    if (temp1.Equals(temp2))
                    {
                        continue;
                    }

                    if (temp1.TheSame(temp2))
                    {
                        res.Remove(temp1);
                        res.Remove(temp2);
                        res.Add(temp1 + temp2);
                        usedLeft.Add(temp1);
                        continue;
                    }
                    res.Add(temp2);
                }

                if (!usedLeft.Contains(temp1))
                {
                    res.Add(temp1);
                }
            }

            usedLeft.Clear();
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Normilize()
        {
            var replaces = new Dictionary<AlgebraicSummand, HashSet<AlgebraicSummand>>();
            foreach (var summand in Summands)
            {
                if (summand.TryNormilize())
                {
                    replaces.TryAdd(summand, summand.Summands);
                }
            }

            foreach (var replace in replaces)
            {
                // FIXME: после выполнения TryNormilize элемент в хеше меняется и удаление за n(1) уже не работает
                var temp = Summands.ToList();
                if (temp.Remove(replace.Key))
                {
                    foreach (var v in replace.Value)
                    {
                        temp.Add(v);
                    }

                    Summands = temp.ToHashSet();
                }
            }

            Summands = Summands.OrderByDescending(summand => summand.Priority).ToHashSet();
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