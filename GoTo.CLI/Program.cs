using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoTo.CLI
{
    static class Program
    {
        const string BuildOption = "build";
        const string RunOption = "run";

        static void Main(string[] args)
        {
            string option;

            if (args.Length < 1)
            {
                PrintOutterUsage();
                return;
            }

            option = args[0];

            switch (option)
            {
                case BuildOption:
                    Build(args.Skip(1).ToArray());
                    break;
                case RunOption:
                    Run(args.Skip(1).ToArray());
                    break;
                default:
                    PrintOutterUsage();
                    break;
            }
        }

        static void Build(string[] args)
        {
            string inputFile, programName;

            if (args.Length < 2)
            {
                Print($"Usage: GoTo.CLI.exe {nameof(inputFile)} {nameof(programName)}");
                Print($"{nameof(inputFile)}: a GOTO file —something like HelloWorld.goto, for instance");
                Print(
                    $"{nameof(programName)}: a valid name for the resulting program —something like CopyInput. " +
                    "It will be used too as the output assembly filename —i.e. CopyInput.dll");
                return;
            }

            inputFile = args[0];

            if (!File.Exists(inputFile))
            {
                Print($"{inputFile} doesn't exist, please double-check");
            }

            var inputStream = File.OpenText(inputFile);
            programName = args[1];
            var outputPath = $"{programName}.dll";

            var messages = Compiler.Build(inputStream, programName, outputPath);

            Print(messages);

            if (!messages.Any(item => item.Severity == SeverityEnum.Error))
            {
                Print("Success!");
            }
        }

        static void Run(string[] args)
        {
            string assemblyFilename, programName, x1;

            if (args.Length < 2)
            {
                Print(
                    $"Usage: GoTo.CLI.exe {nameof(RunOption)} " +
                    $"{nameof(assemblyFilename)} {nameof(programName)} {nameof(x1)}");
                return;
            }

            assemblyFilename = args[0];
            var assembly = Assembly.LoadFile(assemblyFilename);
            programName = args[1];
            var type = assembly.GetType($"GoTo.{programName}");
            x1 = args[2];
            var actualX1 = Int32.Parse(x1);
            var result = (int)type
                .GetMethod("Run")
                .Invoke(null, new object[] { actualX1, 0, 0, 0, 0, 0, 0, 0 });
            Print($"Result: {result}");
        }

        static void Print(string message)
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

        static void PrintOutterUsage(string option = null)
        {
            Print($"Usage: GoTo.CLI.exe {nameof(option)} ...");
            Print($"{nameof(option)}: {BuildOption}, {RunOption}");
        }
    }
}
