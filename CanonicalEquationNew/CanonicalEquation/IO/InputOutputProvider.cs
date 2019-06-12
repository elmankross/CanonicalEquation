using System.Collections.Generic;

namespace CanonicalEquation.IO
{
    public interface IInputOutputProvider
    {
        IEnumerable<string> Read();
        void Write(string line);
    }
}