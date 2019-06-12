using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class SummandsEnumerator : IEnumerator<string>
    {
        public string Current { get; private set; }
        object IEnumerator.Current => Current;

        private readonly StringBuilder _buffer;
        private readonly string _input;
        private int _currentIndex;

        public SummandsEnumerator(string input)
        {
            _buffer = new StringBuilder(input.Length);
            _input = input;
            _currentIndex = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            for (; _currentIndex < _input.Length; _currentIndex++)
            {
                var currentSymbol = _input[_currentIndex];

                if (currentSymbol == Symbols.OPEN_BRACKET)
                {
                    var endBlockIndex = _input.LastIndexOf(Symbols.CLOSE_BRACKET);
                    _buffer.Append(_input.Substring(_currentIndex, endBlockIndex - _currentIndex));
                    Current = _buffer.ToString();
                    _buffer.Clear();
                    return true;
                }

                if (currentSymbol == Symbols.MINUS || currentSymbol == Symbols.PLUS)
                {
                    if (_buffer.Length != 0)
                    {
                        Current = _buffer.ToString();
                        _buffer.Clear();
                        return true;
                    }
                }

                _buffer.Append(currentSymbol);
            }

            if (_buffer.Length != 0)
            {
                Current = _buffer.ToString();
                _buffer.Clear();
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _buffer.Clear();
            _currentIndex = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Reset();
        }
    }
}
