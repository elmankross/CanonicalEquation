using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class TrimerEnumerator<TModel> : IEnumerator<string>
    {
        public int CurrentIndex { get; private set; }
        public TModel CurrentObject { get; private set; }
        public string Current { get; private set; }
        object IEnumerator.Current => Current;

        private bool _iterating;
        private readonly bool _autotrim;
        private readonly TModel[] _models;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        /// <param name="autotrim"></param>
        public TrimerEnumerator(ICollection<TModel> models, bool autotrim = true)
        {
            _iterating = false;
            CurrentIndex = 0;
            _models = models.ToArray();
            _autotrim = autotrim;
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimBeginMathSymbol()
        {
            TrimBeginPlusSymbol();
            TrimBeginMinusSymbol();
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimBeginPlusSymbol()
        {
            Current = Current.StartsWith(Symbols.PLUS)
                ? Current.Remove(0, 1)
                : Current;
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimBeginMinusSymbol()
        {
            Current = Current.StartsWith(Symbols.MINUS)
                ? Current.Remove(0, 1)
                : Current;
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimBeginZero()
        {
            Current = Current.StartsWith("0") ? Current.Remove(0, 1) : Current;
        }


        /// <summary>
        /// 
        /// </summary>
        private void Trim()
        {
            if (CurrentIndex == 0)
            {
                TrimBeginMathSymbol();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (_iterating)
            {
                CurrentIndex++;
            }

            for (; CurrentIndex < _models.Length;)
            {
                CurrentObject = _models[CurrentIndex];
                Current = CurrentObject.ToString();
                if (_autotrim)
                {
                    Trim();
                }

                _iterating = true;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            CurrentIndex = 0;
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
