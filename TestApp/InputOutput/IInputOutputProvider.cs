using System.Collections.Generic;

namespace TestApp.InputOutput
{
    public interface IInputOutputProvider
    {
        IEnumerable<string> ReadInput();
        void WriteOutput(string result);
    }
}
