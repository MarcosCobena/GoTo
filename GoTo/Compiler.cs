using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GoTo.Features.AbstractSyntaxTree;
using GoTo.Features.CodeGenerator;
using GoTo.Features.Parser;
using GoTo.Features.SemanticAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoTo
{
    public static class Compiler
    {
        public static IEnumerable<Message> Run(string input)
        {
            Analyze(input, out List<Message> messages, out GoToParser.ProgramContext contextSyntaxTree);

            var areThereErrors = messages.Where(message => message.Severity == SeverityEnum.Error).Any();

            if (areThereErrors)
            {
                return messages;
            }

            var abstractSyntaxTreeGenerator = new AbstractSyntaxTreeGenerator();
            var program = abstractSyntaxTreeGenerator.VisitProgram(contextSyntaxTree) as ProgramNode;

            SemanticAnalyzer.CheckLastLineSkip(program, ref messages);

            return messages;
        }

        public static int Run(
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
            Analyze(input, out List<Message> messages, out GoToParser.ProgramContext contextSyntaxTree);
            CheckErrors(input, messages);

            var abstractSyntaxTreeGenerator = new AbstractSyntaxTreeGenerator();
            var program = abstractSyntaxTreeGenerator.VisitProgram(contextSyntaxTree) as ProgramNode;

            SemanticAnalyzer.CheckLastLineSkip(program, ref messages);
            CheckErrors(input, messages);

            var type = CodeGenerator.CreateType(program, "Program");
            var result = (int)type.GetMethod("Run").Invoke(null, new object[] { x1, x2, x3, x4, x5, x6, x7, x8 });

            return result;
        }

        static void CheckErrors(string input, IEnumerable<Message> messages)
        {
            var areThereErrors = messages.Where(message => message.Severity == SeverityEnum.Error).Any();

            if (areThereErrors)
            {
                // TODO leverage messages too
                throw new ArgumentException("The program contains errors.", nameof(input));
            }
        }

        static void Analyze(string input, out List<Message> messages, out GoToParser.ProgramContext contextSyntaxTree)
        {
            var inputStream = CharStreams.fromstring(input);

            var lexer = new GoToLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.AddErrorListener(lexerErrorListener);
            messages = new List<Message>();
            messages.AddRange(lexerErrorListener.Messages);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);
            contextSyntaxTree = parser.program();
            messages.AddRange(parserErrorListener.Messages);

            var listener = new SemanticListener();
            ParseTreeWalker.Default.Walk(listener, contextSyntaxTree);
            messages.AddRange(listener.Messages);
        }
    }
}
