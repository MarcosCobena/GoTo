using System;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GoTo.Features.SemanticAnalyzer;

namespace GoTo
{
    public static class Compiler
    {
        public static Assembly Run(string input)
        {
            var inputStream = CharStreams.fromstring(input);
            var lexer = new GoToLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            IParseTree parseTree = parser.program();

            var listener = new SemanticListener();
            ParseTreeWalker.Default.Walk(listener, parseTree);

            throw new NotImplementedException();
        }
    }
}
