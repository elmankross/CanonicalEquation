using System;
using System.Collections.Generic;
using App;
using App.IO;

namespace CanonicalEquation.IO
{
    public abstract class InputOutputProvider
    {
        public static InputOutputProvider Get(ConsoleArgs cfg)
        {
            switch (cfg.Mode)
            {
                case Mode.Interactive:
                    return new ConsoleInputOutputProvider();
                case Mode.File:
                    return new FileInputOutputProvider(cfg.FilePath);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cfg.Mode), cfg.Mode, null);
            }
        }

        public abstract IEnumerable<string> Read();
        public abstract void Write(string line);
    }
}