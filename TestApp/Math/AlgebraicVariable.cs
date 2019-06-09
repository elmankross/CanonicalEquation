using System.Text;

namespace TestApp.Math
{
    public class AlgebraicVariable
    {
        public int Power { get; }
        public char Name { get; }

        public AlgebraicVariable(char name, int power = 1)
        {
            Name = name;
            Power = power;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            if (Power != 1)
            {
                sb.Append(Symbols.Pwd);
                sb.Append(Power);
            }
            return sb.ToString();
        }
    }
}
