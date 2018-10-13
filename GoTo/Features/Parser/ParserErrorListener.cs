using Antlr4.Runtime;
using System.IO;

namespace GoTo.Features.Parser
{
    class ParserErrorListener : IAntlrErrorListener<IToken>
    {
        public void SyntaxError(
            TextWriter output, 
            IRecognizer recognizer, 
            IToken offendingSymbol, 
            int line, 
            int charPositionInLine, 
            string msg, 
            RecognitionException e)
        {
            throw new System.NotImplementedException();
        }
    }
}