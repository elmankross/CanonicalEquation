using System.Linq;

namespace TestApp.Math
{
    public static class Symbols
    {
        public static readonly char Empty = '\0';
        public static readonly char Equal = '=';
        public static readonly char OpenBracket = '(';
        public static readonly char CloseBracket = ')';
        public static readonly char Plus = '+';
        public static readonly char Minus = '-';
        public static readonly char Pwd = '^';

        public static readonly char[] AllowedMathOperators = { Plus, Minus };
        public static readonly char[] AllowedBrackets = { OpenBracket, CloseBracket };
        public static readonly char[] AllowedFloatPoints = { '.' };
        public static readonly char[] AllowedPwd = { Pwd };

        public static char[] AllowedSymbols =
            AllowedMathOperators.Concat(AllowedBrackets)
                                .Concat(AllowedFloatPoints)
                                .Concat(AllowedPwd)
                                .ToArray();
    }
}
