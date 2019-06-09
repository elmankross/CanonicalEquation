using Microsoft.Extensions.Configuration;
using TestApp.InputOutput;
using TestApp.Math;

namespace TestApp
{
    class Program
    {
        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder().AddCommandLine(args).Build();

            var ioProvider = _configuration.GetIOProvider();

            foreach (var input in ioProvider.ReadInput())
            {
                if (AlgebraicEquation.TryParse(input, out var equation, out var error))
                {
                    equation.ToCanonicalForm();
                    ioProvider.WriteOutput(equation.ToString());
                }
                else
                {
                    ioProvider.WriteOutput(error);
                }
            }
        }
    }
}
