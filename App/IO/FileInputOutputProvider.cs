using System.Collections.Generic;
using System.IO;
using CanonicalEquation.IO;

namespace App.IO
{
    internal class FileInputOutputProvider : InputOutputProvider
    {
        private readonly string _dstFilePath;
        private readonly string _srcFilePath;

        internal FileInputOutputProvider(string filePath)
        {
            _srcFilePath = filePath;
            _dstFilePath = _srcFilePath + ".out";
        }


        public override IEnumerable<string> Read()
        {
            return File.ReadLines(_srcFilePath);
        }


        public override void Write(string line)
        {
            File.AppendAllLines(_dstFilePath, new[] { line });
        }
    }
}
