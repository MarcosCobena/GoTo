using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GoTo.Emitter;
using GoTo.Parser;
using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoTo
{
    public static class Language
    {
        const int ErrorResult = 0;

        public const string OutputMethodName = "Run";
        public const string OutputNamespace = "GoTo";

        public static ProgramNode Analyze(string input, out List<Message> messages)
        {
            messages = new List<Message>();

            var expandedInput = ExpandMacros(input, messages);

            var inputStream = CharStreams.fromstring(expandedInput);

            var lexer = new GoToLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.AddErrorListener(lexerErrorListener);
            messages.AddRange(lexerErrorListener.Messages);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);
            var contextSyntaxTree = parser.program();
            messages.AddRange(parserErrorListener.Messages);

            var listener = new SemanticListener();
            ParseTreeWalker.Default.Walk(listener, contextSyntaxTree);
            messages.AddRange(listener.Messages);

            if (AreThereErrors(messages))
            {
                return null;
            }

            var abstractSyntaxTreeGenerator = new AbstractSyntaxTreeGenerator();
            var program = abstractSyntaxTreeGenerator.VisitProgram(contextSyntaxTree) as ProgramNode;

            SemanticAnalyzer.CheckUnknownInput(program, ref messages);
            SemanticAnalyzer.CheckLastLineSkip(program, ref messages);
            SemanticAnalyzer.CheckMissingLabel(program, ref messages);

            if (AreThereErrors(messages))
            {
                return null;
            }
            else
            {
                return program;
            }
        }

        public static IEnumerable<Message> Build(StreamReader inputStream, string programName, string outputPath)
        {
            var input = inputStream.ReadToEnd();
            var result = Compile(input, programName, outputPath);

            return result.messages;
        }

        public static (int result, IEnumerable<Message> messages) Run(
            string input,
            int x1 = 0, 
            int x2 = 0, 
            int x3 = 0, 
            int x4 = 0, 
            int x5 = 0, 
            int x6 = 0, 
            int x7 = 0, 
            int x8 = 0)
        {
            var output = Compile(input);

            if (AreThereErrors(output.messages))
            {
                return (0, output.messages);
            }

            var result = (int)output.result.GetMethod(OutputMethodName)
                .Invoke(null, new object[] { x1, x2, x3, x4, x5, x6, x7, x8 });
            
            return (result, output.messages);
        }

        static bool AreThereErrors(IEnumerable<Message> messages) => 
            messages.Any(message => message.Severity == SeverityEnum.Error);

        static (Type result, IEnumerable<Message> messages) Compile(
            string input, string programName = "Program", string outputPath = null)
        {
            var program = Analyze(input, out List<Message> messages);

            if (AreThereErrors(messages))
            {
                return (null, messages);
            }

            if (outputPath == null)
            {
                var type = ILEmitter.CreateType(program, programName);

                return (type, messages);
            }
            else
            {
                ILEmitter.CreateAssembly(program, programName, outputPath);

                return (null, messages);
            }
        }

        static string ExpandMacros(string input, List<Message> messages)
        {
            var inputStream = CharStreams.fromstring(input);

            var lexer = new GoToLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.AddErrorListener(lexerErrorListener);
            messages.AddRange(lexerErrorListener.Messages);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);
            var contextSyntaxTree = parser.program();
            messages.AddRange(parserErrorListener.Messages);

            var listener = new MacroExpansionListener(tokenStream);
            ParseTreeWalker.Default.Walk(listener, contextSyntaxTree);
            messages.AddRange(listener.Messages);

            var expandedInput = listener.RewrittenTokenStream.GetText();

            return expandedInput;
        }
    }
}
