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
            for (; _currentIndex < _input.Length ; _currentIndex++)
            {
                var currentSymbol = _input[_currentIndex];

                if (currentSymbol == Symbols.OPEN_BRACKET)
                {
                    var endBlockIndex = _input.LastIndexOf(Symbols.CLOSE_BRACKET) + 1;
                    _buffer.Append(_input.Substring(_currentIndex, endBlockIndex - _currentIndex));
                    _currentIndex = endBlockIndex;
                    return IndicateFound();
                }

                if (currentSymbol == Symbols.MINUS || currentSymbol == Symbols.PLUS)
                {
                    var prevSymbol = _input[_currentIndex - 1];
                    if (prevSymbol != Symbols.POWER)
                    {
                        if (_buffer.Length != 0)
                        {
                            return IndicateFound();
                        }
                    }
                }

                _buffer.Append(currentSymbol);
            }

            if (_buffer.Length != 0)
            {
                return IndicateFound();
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IndicateFound()
        {
            Current = _buffer.ToString();
            _buffer.Clear();
            return true;
        }
    }
}
