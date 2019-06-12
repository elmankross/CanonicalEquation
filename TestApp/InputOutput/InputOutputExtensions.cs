using System;
using Microsoft.Extensions.Configuration;

namespace TestApp.InputOutput
{
    public static class InputOutputExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IInputOutputProvider GetIOProvider(this IConfiguration config)
        {
            switch (config["mode"])
            {
                case "file":
                    return new FileInputOutputProvider(config["path"]);

                case "interactive":
                    return new ConsoleInputOutputProvider();

                default:
                    Console.WriteLine("USAGE: [options]\n"
                                    + "OPTIONS:\n"
                                    + " /mode\n"
                                    + "     file        - process data from file and send result to another one\n"
                                    + "     interactive - use interactive mode to communicate with app\n"
                                    + " /path           - point out a file path in 'file' mode");
                    throw new NotImplementedException("Not supported mode! Supported are 'file' and 'interactive'");
            }
        }
    }
}
