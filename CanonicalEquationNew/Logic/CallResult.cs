using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public class CallResult
    {
        private readonly StringBuilder _buffer;
        private readonly HashSet<string> _errors;
        public bool IsSuccessfull => !_errors.Any();

        public CallResult()
        {
            _errors = new HashSet<string>(5);
            _buffer = new StringBuilder();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="input"></param>
        public void AddError(string message, string input = null)
        {
            if (input != null)
            {
                _buffer.Append('\'');
                _buffer.Append(input);
                _buffer.Append("\': ");
            }

            _buffer.Append(message);
            _errors.Add(_buffer.ToString());
            _buffer.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        public void CopyErrorsTo(CallResult destination)
        {
            foreach (var error in _errors)
            {
                destination._errors.Add(error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetErrorsString()
        {
            return string.Join("\n", _errors);
        }
    }
}
