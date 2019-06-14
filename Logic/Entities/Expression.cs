using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Entities
{
    public class Expression
    {
        public HashSet<Summand> Summands { get; }

        private readonly StringBuilder _buffer;

        private Expression()
        {
            Summands = new HashSet<Summand>();
            _buffer = new StringBuilder();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CallResult TryParse(string input, out Expression expression)
        {
            expression = new Expression();
            var result = new CallResult();

            if (string.IsNullOrEmpty(input))
            {
                result.AddError("Expression is empty");
                return result;
            }

            input = input.Replace(" ", string.Empty).Replace("\t", string.Empty);

            var summandsEnumerator = new SummandsEnumerator(input);
            while (summandsEnumerator.MoveNext())
            {
                var parseResult = Summand.TryParse(summandsEnumerator.Current, out var summand);
                if (!parseResult.IsSuccessfull)
                {
                    parseResult.CopyErrorsTo(result);
                    return result;
                }

                expression.Summands.Add(summand);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression operator -(Expression left, Expression right)
        {
            var result = new Expression();

            foreach (var summand in left.Summands.Select(s => s.Clone()))
            {
                result.Summands.Add(summand as Summand);
            }

            foreach (var summand in right.Summands.Select(s => s.Clone()))
            {
                var res = summand as Summand;
                res.Invert();
                result.Summands.Add(res);
            }

            return result;
        }


        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            _buffer.Clear();

            var summandEnumerator = new TrimerEnumerator<Summand>(Summands, false);
            while (summandEnumerator.MoveNext())
            {
                if (summandEnumerator.CurrentIndex == 0)
                {
                    summandEnumerator.TrimBeginPlusSymbol();
                }
                else
                {
                    if (summandEnumerator.CurrentObject.Multiplier.Equals(0))
                    {
                        summandEnumerator.TrimBeginZero();
                    }
                }

                _buffer.Append(summandEnumerator.Current);
            }

            return _buffer.ToString();
        }

        #endregion
    }
}
