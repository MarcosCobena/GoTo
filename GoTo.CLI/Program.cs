using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoTo.CLI
{
    static class Program
    {
        static void Main(string[] args)
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
    }
}
