using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GoTo.Features.AbstractSyntaxTree;
using GoTo.Features.Parser;
using GoTo.Features.SemanticAnalyzer;
using System.Collections.Generic;
using System.Linq;

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
            var messages = new List<Message>();
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

            var areThereErrors = messages.Where(message => message.Severity == SeverityEnum.Error).Any();

            if (!areThereErrors)
            {
                var abstractSyntaxTreeGenerator = new AbstractSyntaxTreeGenerator();
                var program = abstractSyntaxTreeGenerator.VisitProgram(contextSyntaxTree);
            }

            return messages;
        }
    }
}
