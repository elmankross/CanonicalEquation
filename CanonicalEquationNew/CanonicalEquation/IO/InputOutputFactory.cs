using System;
using CanonicalEquation.IO;

namespace App.IO
{
    public static class InputOutputFactory
    {
        public static IInputOutputProvider Get(ConsoleArgs cfg)
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
    }
}
