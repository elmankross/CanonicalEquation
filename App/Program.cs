using App;
using CanonicalEquation.IO;
using Logic.Entities;
using Microsoft.Extensions.Configuration;

namespace CanonicalEquation
{
    internal class Program
    {
        private static ConsoleArgs _configuration;

        private static void Main(string[] args)
        {
            _configuration = new ConsoleArgs();
            new ConfigurationBuilder().AddCommandLine(args)
                                      .Build()
                                      .Bind(_configuration);

            var ioProvider = InputOutputProvider.Get(_configuration);
            foreach (var line in ioProvider.Read())
            {
                var parseResult = Equation.TryParse(line, out var equation);
                if (parseResult.IsSuccessfull)
                {
                    var canonicalForm = equation.GetCanonicalForm();
                    ioProvider.Write(canonicalForm.ToString());
                }
                else
                {
                    ioProvider.Write(parseResult.GetErrorsString());
                }
            }
        }
    }
}
