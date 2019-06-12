using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestApp.Math;

namespace TestApp
{
    public class ExpressionEnumerator : IEnumerator<string>
    {
        private int _currentIndex;
        private readonly StringBuilder _partBuilder;
        private readonly string _expression;

        public ExpressionEnumerator(string expression)
        {
            _currentIndex = 0;
            _expression = expression;
            _partBuilder = new StringBuilder(expression.Length);
        }

        #region Implementation of IEnumerator

        /// <inheritdoc />
        public bool MoveNext()
        {
            for (; _currentIndex < _partBuilder.Capacity ; _currentIndex++)
            {
                var currentSymbol = _expression[_currentIndex];

                if (currentSymbol.Equals(Symbols.OpenBracket))
                {
                    var lastBracketIndex = _expression.LastIndexOf(Symbols.CloseBracket) + 1;
                    _partBuilder.Append(_expression.Substring(_currentIndex, lastBracketIndex - _currentIndex));
                    Current = _partBuilder.ToString();
                    _partBuilder.Clear();
                    _currentIndex = lastBracketIndex;
                    return true;
                }

                if (Symbols.AllowedMathOperators.Contains(currentSymbol))
                {
                    if (_partBuilder.Length != 0)
                    {
                        Current = _partBuilder.ToString();
                        _partBuilder.Clear();
                        return true;
                    }
                }

                _partBuilder.Append(currentSymbol);
            }

            if (_partBuilder.Length != 0)
            {
                Current = _partBuilder.ToString();
                _partBuilder.Clear();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _currentIndex = 0;
            _partBuilder.Clear();
        }

        /// <inheritdoc />
        public string Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Reset();
        }

        #endregion
    }
}