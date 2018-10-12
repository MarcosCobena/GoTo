using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using static GoToParser;

namespace GoTo.Features.SemanticAnalyzer
{
    public class SemanticListener : GoToBaseListener
    {
        readonly IList<Message> _messages;

        public SemanticListener()
        {
            _messages = new List<Message>();
        }

        public IEnumerable<Message> Messages => _messages;

        public override void ExitUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context)
        {
            base.ExitUnaryExpression(context);

            CheckVarsAreEqual(
                (context.Parent as InstructionContext).Start, 
                context.VAR().Symbol,
                "Skip instruction must have the same var at both sides.");
        }

        public override void ExitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context)
        {
            base.ExitBinaryExpression(context);

            CheckVarsAreEqual(
                (context.Parent as InstructionContext).Start, 
                context.VAR().Symbol,
                "Increment and decrement instructions must have the same var at both sides.");
        }

        void CheckVarsAreEqual(IToken leftSideToken, IToken rightSideToken, string errorDescription)
        {
            var leftSideVar = leftSideToken.Text;
            var rightSideVar = rightSideToken.Text;

            if (!rightSideVar.Equals(leftSideVar))
            {
                var message = new Message(
                    SeverityEnum.Error, errorDescription, rightSideToken.Line, rightSideToken.Column);
                _messages.Add(message);
            }
        }
    }
}
