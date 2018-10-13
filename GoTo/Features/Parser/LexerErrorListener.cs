using Antlr4.Runtime;
using System.IO;

namespace GoTo.Features.Parser
{
    class LexerErrorListener : IAntlrErrorListener<int>
    {
        public void SyntaxError(
            TextWriter output, 
            IRecognizer recognizer, 
            int offendingSymbol, 
            int line, 
            int charPositionInLine, 
            string msg, 
            RecognitionException e)
        {
            throw new System.NotImplementedException();
        }
    }
}