using System;
using System.Collections.Generic;
using CanonicalEquation.IO;

namespace App.IO
{
    internal class ConsoleInputOutputProvider : InputOutputProvider
    {
        private bool _isCanceled;

        internal ConsoleInputOutputProvider()
        {
            Console.CancelKeyPress += (_, args) => { _isCanceled = args.Cancel = true; };
        }


        public override IEnumerable<string> Read()
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


        public override void Write(string line)
        {
            Console.Write("Result: ");
            Console.WriteLine(line);
            Console.WriteLine();
        }
    }
}
