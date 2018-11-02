using System;
using System.Linq;

namespace GoTo.CLI
{
    static class Program
    {
        internal const string Selfname = "gotool.exe";
        internal const string BuildOption = "build";
        internal const string RunOption = "run";

        static void Main(string[] args)
        {
            Printer.PrintHeader();

            string option;

            if (args.Length < 1)
            {
                Printer.PrintOutterUsage();
                return;
            }

            option = args[0];
            var restOfArgs = args.Skip(1).ToArray();

            try
            {
                switch (option)
                {
                    case BuildOption:
                        Options.Build(restOfArgs);
                        break;
                    case RunOption:
                        Options.Run(restOfArgs);
                        break;
                    default:
                        Printer.PrintOutterUsage();
                        break;
                }
            }
            catch (Exception exception)
            {
                Printer.Print($"Error: {exception}");
            }
        }
    }
}
