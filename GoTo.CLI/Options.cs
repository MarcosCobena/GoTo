using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoTo.CLI
{
    static class Options
    {
        internal static void Build(string[] args)
        {
            string inputFilename = null;

            if (args.Length < 1)
            {
                Printer.PrintBuildUsage(inputFilename);
                return;
            }

            inputFilename = args[0];
            CheckFileExists(inputFilename);
            var programName = Path.GetFileNameWithoutExtension(inputFilename);
            var outputFilename = $"{programName}.dll";

            using (var inputStream = File.OpenText(inputFilename))
            {
                var messages = Language.Build(inputStream, programName, outputFilename);

                Printer.Print(messages);

                if (!messages.Any(item => item.Severity == SeverityEnum.Error))
                {
                    Printer.Print($"Success! {outputFilename}");
                }
            }
        }

        internal static void Run(string[] args)
        {
            string assemblyFilename = null;
            string x1 = null;

            if (args.Length < 2)
            {
                Printer.PrintRunUsage(assemblyFilename, x1);
                return;
            }

            assemblyFilename = args[0];
            CheckFileExists(assemblyFilename);
            var fullPath = Path.GetFullPath(assemblyFilename);
            // Microsoft's implementation barks for the full path; however, Mono loads it just fine
            var assembly = Assembly.LoadFile(fullPath);

            var programName = Path.GetFileNameWithoutExtension(assemblyFilename);
            var type = assembly.GetType($"{Language.OutputNamespace}.{programName}");

            x1 = args[1];
            var actualX1 = int.Parse(x1);

            var result = (int)type
                .GetMethod(Language.OutputMethodName)
                .Invoke(null, new object[] { actualX1, 0, 0, 0, 0, 0, 0, 0 });
            Printer.Print(result.ToString());
        }

        static void CheckFileExists(string inputFile)
        {
            if (!File.Exists(inputFile))
            {
                throw new ArgumentException($"The file {inputFile} doesn't exist", nameof(inputFile));
            }
        }
    }
}
