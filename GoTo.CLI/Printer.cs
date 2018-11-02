using System;
using System.Collections.Generic;

namespace GoTo.CLI
{
    static class Printer
    {
        internal static void PrintRunUsage(string assemblyFilename, string x1)
        {
            Print(
                $"Usage: {Program.Selfname} {Program.RunOption} " +
                $"{nameof(assemblyFilename)} {nameof(x1)}");
        }

        internal static void Print(string message = "")
        {
            Console.WriteLine(message);
        }

        internal static void Print(IEnumerable<Message> messages)
        {
            foreach (var item in messages)
            {
                Print($"{item.Severity} at line {item.Line}, column {item.Column}: {item.Description}");
            }
        }

        internal static void PrintBuildUsage(string inputFile)
        {
            Print($"Usage: {Program.Selfname} {Program.BuildOption} {nameof(inputFile)}");
            Print($"- {nameof(inputFile)}: a GOTO file —something like HelloWorld.goto, for instance");
        }

        internal static void PrintHeader()
        {
            Print($"GOTO-ol");
            Print();
        }

        internal static void PrintOutterUsage(string option = null)
        {
            Print($"Usage: {Program.Selfname} {nameof(option)} ...");
            Print($"- {nameof(option)}: {Program.BuildOption}, {Program.RunOption}");
        }
    }
}
