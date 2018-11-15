using Antlr4.Runtime;
using System.Collections.Generic;
using System.IO;

namespace GoTo.Parser
{
    class ParserErrorListener : IAntlrErrorListener<IToken>
    {
        List<Message> _messages;

        public ParserErrorListener()
        {
            _messages = new List<Message>();
        }

        public IEnumerable<Message> Messages => _messages;

        public void SyntaxError(
            TextWriter output, 
            IRecognizer recognizer, 
            IToken offendingSymbol, 
            int line, 
            int charPositionInLine, 
            string msg, 
            RecognitionException e)
        {
            var message = new Message(SeverityEnum.Error, msg, line, charPositionInLine);
            _messages.Add(message);
        }
    }
}