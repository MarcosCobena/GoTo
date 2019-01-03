using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GoTo.Emitter;
using GoTo.Interpreter;
using GoTo.Parser;
using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoTo
{
    public static class Framework
    {
        const string DefaultProgramName = "Program";
        const int DefaultProgramOutput = 0;

        public const string OutputMethodName = "Run";
        public const string OutputNamespace = "GoTo";

        public static bool TryAnalyze(string input, out ProgramNode program, out IEnumerable<Message> messages)
        {
            program = null;
            var analysisMessages = new List<Message>();
            messages = analysisMessages;

            TryExpandMacros(input, out string expandedInput, out IEnumerable<Message> expasionMessages);
            analysisMessages.AddRange(expasionMessages);

            var inputStream = CharStreams.fromstring(expandedInput);

            var lexer = new GoToLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.AddErrorListener(lexerErrorListener);
            analysisMessages.AddRange(lexerErrorListener.Messages);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);
            var contextSyntaxTree = parser.program();
            analysisMessages.AddRange(parserErrorListener.Messages);

            var listener = new SemanticListener();
            ParseTreeWalker.Default.Walk(listener, contextSyntaxTree);
            analysisMessages.AddRange(listener.Messages);

            if (AreThereErrors(messages))
            {
                return false;
            }

            var abstractSyntaxTreeGenerator = new AbstractSyntaxTreeGenerator();
            program = abstractSyntaxTreeGenerator.VisitProgram(contextSyntaxTree) as ProgramNode;

            SemanticAnalyzer.Check(program, ref analysisMessages);

            if (AreThereErrors(messages))
            {
                return false;
            }

            return true;
        }

        public static bool TryBuild(
            StreamReader inputStream, 
            string programName, 
            string outputPath, 
            out IEnumerable<Message> messages)
        {
            var input = inputStream.ReadToEnd();
            var isSuccess = TryCompile(
                input, 
                out Type type, 
                out IEnumerable<Message> compilationMessages, 
                programName, 
                outputPath);
            messages = compilationMessages;

            if (!isSuccess || AreThereErrors(messages))
            {
                return false;
            }

            return true;
        }

        public static bool TryRun(
            string input,
            out int result, 
            out IEnumerable<Message> messages,
            int x1 = 0, 
            int x2 = 0, 
            int x3 = 0, 
            int x4 = 0, 
            int x5 = 0, 
            int x6 = 0, 
            int x7 = 0, 
            int x8 = 0,
            bool isInterpreted = false)
        {
            result = DefaultProgramOutput;

            if (isInterpreted)
            {
                var isSuccess = TryAnalyze(input, out ProgramNode program, out IEnumerable<Message> analysisMessages);
                messages = analysisMessages;

                if (!isSuccess || AreThereErrors(analysisMessages))
                {
                    return false;
                }

                result = VirtualMachine.Run(program, x1, x2, x3, x4, x5, x6, x7, x8);
            }
            else
            {
                var isSuccess = TryCompile(input, out Type type, out IEnumerable<Message> compilationMessages);
                messages = compilationMessages;

                if (!isSuccess || AreThereErrors(compilationMessages))
                {
                    return false;
                }

                result = (int)type
                    .GetMethod(OutputMethodName)
                    .Invoke(null, new object[] { x1, x2, x3, x4, x5, x6, x7, x8 });
            }
            
            return true;
        }

        static bool TryCompile(
            string input, 
            out Type type, 
            out IEnumerable<Message> messages, 
            string programName = DefaultProgramName, 
            string outputPath = null)
        {
            var isSuccess = TryAnalyze(input, out ProgramNode program, out IEnumerable<Message> analysisMessages);
            messages = new List<Message>(analysisMessages);

            if (!isSuccess)
            {
                type = null;
                return false;
            }

            // TODO split into different methods?
            if (outputPath == null)
            {
                type = ILEmitter.CreateType(program, programName);
            }
            else
            {
                type = null;
                ILEmitter.CreateAssembly(program, programName, outputPath);
            }

            return true;
        }

        static bool TryExpandMacros(string input, out string output, out IEnumerable<Message> messages)
        {
            var inputStream = CharStreams.fromstring(input);

            var lexer = new GoToLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.AddErrorListener(lexerErrorListener);
            var expansionMessages = new List<Message>(lexerErrorListener.Messages);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);
            var contextSyntaxTree = parser.program();
            expansionMessages.AddRange(parserErrorListener.Messages);

            var listener = new MacroExpansionListener(tokenStream);
            ParseTreeWalker.Default.Walk(listener, contextSyntaxTree);
            expansionMessages.AddRange(listener.Messages);

            output = listener.RewrittenTokenStream.GetText();
            messages = expansionMessages;

            if (AreThereErrors(expansionMessages))
            {
                return false;
            }

            return true;
        }

        static bool AreThereErrors(IEnumerable<Message> messages) =>
            messages.Any(message => message.Severity == SeverityEnum.Error);
    }
}
