using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Logic.Entities
{
    public class Variable : IEquatable<Variable>
    {
        public char Name { get; }
        public int Power { get; }

        private const string NAME_GROUP = "name";
        private const string POWER_GROUP = "power";
        private static readonly Regex Pattern = new Regex($"^(?<{NAME_GROUP}>\\w)(\\^(?<{POWER_GROUP}>[\\-\\d]*))?$", RegexOptions.Compiled);
        private readonly StringBuilder _buffer;

        private Variable(char name, int power)
        {
            _buffer = new StringBuilder(3);

            Name = name;
            Power = power;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static CallResult TryParse(string input, out Variable variable)
        {
            variable = null;
            var result = new CallResult();
            if (!Pattern.IsMatch(input))
            {
                result.AddError("Invalid variable format", input);
                return result;
            }

            var regex = Pattern.Match(input).Groups;

            var power = regex[POWER_GROUP].Value;
            variable = new Variable(
                regex[NAME_GROUP].Value[0],
                !string.IsNullOrEmpty(power) ? int.Parse(power) : 1);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            _buffer.Clear();

            if (Power == 0)
            {
                return _buffer.ToString();
            }

            _buffer.Append(Name);

            if (Power == 1)
            {
                return _buffer.ToString();
            }

            _buffer.Append("^");
            _buffer.Append(Power);
            return _buffer.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Variable other)
        {
            return Name.Equals(other.Name)
                && Power.Equals(other.Power);
        }


        /// <summary>
        /// For <see cref="System.Collections.Generic.HashSet{T}"/> compare
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Variable operator *(Variable left, Variable right)
        {
            if (left.Name != right.Name)
            {
                throw new NotSupportedException("Cannot multiply variables with different names");
            }

            return new Variable(left.Name, left.Power + right.Power);
        }
    }
}
