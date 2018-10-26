using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoTo.CLI
{
    static class Program
    {
        const string Selfname = "gotor.exe";
        const string BuildOption = "build";
        const string RunOption = "run";

        static void Main(string[] args)
        {
            PrintHeader();

            string option;

            if (args.Length < 1)
            {
                PrintOutterUsage();
                return;
            }

            option = args[0];
            var restOfArgs = args.Skip(1).ToArray();

            try
            {
                switch (option)
                {
                    case BuildOption:
                        Build(restOfArgs);
                        break;
                    case RunOption:
                        Run(restOfArgs);
                        break;
                    default:
                        PrintOutterUsage();
                        break;
                }
            }
            catch (Exception exception)
            {
                Print($"Error: {exception}");
            }
        }

        static void Build(string[] args)
        {
            string inputFilename = null;

            if (args.Length < 1)
            {
                PrintBuildUsage(inputFilename);
                return;
            }

            inputFilename = args[0];
            CheckFileExists(inputFilename);
            var programName = Path.GetFileNameWithoutExtension(inputFilename);
            var outputFilename = $"{programName}.dll";

            using (var inputStream = File.OpenText(inputFilename))
            {
                var messages = Compiler.Build(inputStream, programName, outputFilename);

                Print(messages);

                if (!messages.Any(item => item.Severity == SeverityEnum.Error))
                {
                    Print($"Success! {outputFilename}");
                }
            }
        }

        static void Run(string[] args)
        {
            string assemblyFilename = null;
            string x1 = null;

            if (args.Length < 1)
            {
                PrintRunUsage(assemblyFilename, x1);
                return;
            }

            assemblyFilename = args[0];
            CheckFileExists(assemblyFilename);
            var fullPath = Path.GetFullPath(assemblyFilename);
            // Microsoft's implementation barks for the full path; however, Mono loads it just fine
            var assembly = Assembly.LoadFile(fullPath);

            var programName = Path.GetFileNameWithoutExtension(assemblyFilename);
            var type = assembly.GetType($"GoTo.{programName}");

            x1 = args[1];
            var actualX1 = int.Parse(x1);

            var result = (int)type
                .GetMethod("Run")
                .Invoke(null, new object[] { actualX1, 0, 0, 0, 0, 0, 0, 0 });
            Print(result.ToString());
        }

        static void CheckFileExists(string inputFile)
        {
            if (!File.Exists(inputFile))
            {
                throw new ArgumentException($"The file {inputFile} doesn't exist", nameof(inputFile));
            }
        }

        private static void PrintRunUsage(string assemblyFilename, string x1)
        {
            Print(
                $"Usage: {Selfname} {RunOption} " +
                $"{nameof(assemblyFilename)} {nameof(x1)}");
        }

        static void Print(string message = "")
        {
            Console.WriteLine(message);
        }

        static void Print(IEnumerable<Message> messages)
        {
            foreach (var item in messages)
            {
                Print($"{item.Severity} at line {item.Line}, column {item.Column}: {item.Description}");
            }
        }

        static void PrintBuildUsage(string inputFile)
        {
            Print($"Usage: {Selfname} {BuildOption} {nameof(inputFile)}");
            Print($"- {nameof(inputFile)}: a GOTO file —something like HelloWorld.goto, for instance");
        }

        static void PrintHeader()
        {
            Print($"GOTO Runtime");
            Print();
        }

        static void PrintOutterUsage(string option = null)
        {
            Print($"Usage: {Selfname} {nameof(option)} ...");
            Print($"- {nameof(option)}: {BuildOption}, {RunOption}");
        }
    }
}
