using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GoTo.Features.Parser;
using GoTo.Features.SemanticAnalyzer;
using System.Collections.Generic;

namespace GoTo
{
    public static class Compiler
    {
        public static IEnumerable<Message> Run(string input)
        {
            var inputStream = CharStreams.fromstring(input);

            var lexer = new GoToLexer(inputStream);
            var lexerErrorListener = new LexerErrorListener();
            lexer.AddErrorListener(lexerErrorListener);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream);
            var parserErrorListener = new ParserErrorListener();
            parser.AddErrorListener(parserErrorListener);

            IParseTree parseTree = parser.program();

            var listener = new SemanticListener();
            ParseTreeWalker.Default.Walk(listener, parseTree);

            return listener.Messages;
        }
    }
}
