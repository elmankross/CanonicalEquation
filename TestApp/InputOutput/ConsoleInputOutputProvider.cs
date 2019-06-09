using System;
using System.Collections.Generic;

namespace TestApp.InputOutput
{
    public class ConsoleInputOutputProvider : IInputOutputProvider
    {
        private bool _isCanceled;

        public ConsoleInputOutputProvider()
        {
            Console.CancelKeyPress += (_, args) =>
            {
                _isCanceled = args.Cancel = true;
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ReadInput()
        {
            while (!_isCanceled)
            {
                Console.Write("Equation: ");
                yield return Console.ReadLine();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void WriteOutput(string result)
        {
            Console.WriteLine("{0}: {1}", "Result", result);
        }
    }
}
