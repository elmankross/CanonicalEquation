using System;
using System.Text;

namespace TestApp.Math
{
    public class AlgebraicVariable : IEquatable<AlgebraicVariable>
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


        #region Equality members

        /// <inheritdoc />
        public bool Equals(AlgebraicVariable other)
        {
            return Power == other.Power
                && Name == other.Name;
        }

        #endregion

        #region Overrides of Object

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion
    }
}