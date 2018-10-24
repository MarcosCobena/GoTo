using System.IO;

namespace GoTo.CLI
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                return;
            }

            var inputFile = args[0];
            var inputStream = File.OpenText(inputFile);
            var programName = args[1];
            var outputPath = args[2];

            var messages = Compiler.Build(inputStream, programName, outputPath);

            //Print(messages);
        }
    }
}
