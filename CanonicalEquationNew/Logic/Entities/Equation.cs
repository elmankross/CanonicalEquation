namespace Logic.Entities
{
    public class Equation
    {
        public Expression LeftExpression { get; private set; }
        public Expression RightExpression { get; private set; }

        private Equation() { }


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


            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Equation GetCanonicalForm()
        {
            return new Equation();
        }
    }
}
