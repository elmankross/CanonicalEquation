using System.Collections.Generic;

namespace Logic.Entities
{
    public class Expression
    {
        public HashSet<Summand> Summands { get; }

        private Expression()
        {
            Summands = new HashSet<Summand>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CallResult TryParse(string input, out Expression expression)
        {
            expression = new Expression();
            var result = new CallResult();

            return result;
        }
    }
}
