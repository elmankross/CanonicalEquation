using System.Collections.Generic;
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


        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            _buffer.Clear();

            var index = 0;
            foreach (var summand in Summands)
            {
                var summandStr = summand.ToString();
                if (index == 0)
                {
                    summandStr = summandStr.StartsWith(Symbols.PLUS) ? summandStr.Remove(0, 1) : summandStr;
                }

                _buffer.Append(summandStr);
                index++;
            }

            return _buffer.ToString();
        }

        #endregion
    }
}
