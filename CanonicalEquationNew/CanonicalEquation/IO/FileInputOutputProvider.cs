using System.Collections.Generic;
using System.IO;
using CanonicalEquation.IO;

namespace App.IO
{
    internal class FileInputOutputProvider : IInputOutputProvider
    {
        private readonly string _dstFilePath;
        private readonly string _srcFilePath;

        internal FileInputOutputProvider(string filePath)
        {
            _srcFilePath = filePath;
            _dstFilePath = _srcFilePath + ".out";
        }


        public IEnumerable<string> Read()
        {
            return File.ReadLines(_srcFilePath);
        }


        public void Write(string line)
        {
            File.AppendAllText(_dstFilePath, line);
        }
    }
}
