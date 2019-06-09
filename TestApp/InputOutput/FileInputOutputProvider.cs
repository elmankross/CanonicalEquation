using System.Collections.Generic;
using System.IO;

namespace TestApp.InputOutput
{
    public class FileInputOutputProvider : IInputOutputProvider
    {
        private readonly string _filePath;

        public FileInputOutputProvider(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found!", filePath);
            }

            _filePath = filePath;
        }

        public IEnumerable<string> ReadInput()
        {
            return File.ReadLines(_filePath);
        }

        public void WriteOutput(string result)
        {
            throw new System.NotImplementedException();
        }
    }
}
