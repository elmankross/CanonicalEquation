using System;
using System.Collections.Generic;
using CanonicalEquation.IO;

namespace App.IO
{
    internal class ConsoleInputOutputProvider : IInputOutputProvider
    {
        private bool _isCanceled;

        internal ConsoleInputOutputProvider()
        {
            Console.CancelKeyPress += (_, args) => { _isCanceled = args.Cancel = true; };
        }


        public IEnumerable<string> Read()
        {
            while (!_isCanceled)
            {
                if (!_isCanceled)
                {
                    Console.Write("Equation: ");
                    yield return Console.ReadLine();
                }
            }
        }


        public void Write(string line)
        {
            Console.Write("Result: ");
            Console.WriteLine(line);
            Console.WriteLine();
        }
    }
}
