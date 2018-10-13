using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using static GoToParser;

namespace GoTo.Features.SemanticAnalyzer
{
    public class SemanticListener : GoToBaseListener
    {
        const string Labels = "ABCDE";
        const char InputVar = 'X';
        const char OutputVar = 'Y';
        const char AuxVar = 'Z';
        const int MinVarLabelIndex = 1;
        const int MaxVarLabelIndex = 8;

        readonly IList<Message> _messages;

        public SemanticListener()
        {
            _messages = new List<Message>();
        }

        public IEnumerable<Message> Messages => _messages;

        public override void ExitUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context)
        {
            base.ExitUnaryExpression(context);

            var leftSideToken = (context.Parent as InstructionContext).Start;
            var rightSideToken = context.VAR().Symbol;

            CheckValidVar(leftSideToken);
            CheckValidVar(rightSideToken);

            CheckVarsAreEqual(leftSideToken, rightSideToken, "Skip instruction must have the same var at both sides.");
        }

        public override void ExitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context)
        {
            base.ExitBinaryExpression(context);

            var leftSideToken = (context.Parent as InstructionContext).Start;
            var rightSideToken = context.VAR().Symbol;

            CheckValidVar(leftSideToken);
            CheckValidVar(rightSideToken);

            CheckVarsAreEqual(
                leftSideToken, 
                rightSideToken,
                "Increment and decrement instructions must have the same var at both sides.");
        }

        public override void ExitConditionalInstruction([NotNull] ConditionalInstructionContext context)
        {
            base.ExitConditionalInstruction(context);

            CheckValidVar(context.VAR().Symbol);
        }

        public override void ExitLabeledLine([NotNull] LabeledLineContext context)
        {
            base.ExitLabeledLine(context);

            CheckValidLabel(context.LABEL().Symbol);
        }

        void CheckValidLabel(IToken token)
        {
            var text = token.Text;
            var letter = text[0];
            var rawIndex = text.Substring(1);

            if (!Labels.Contains(letter.ToString()))
            {
                var message = new Message(
                    SeverityEnum.Error,
                    $"Labels must be one of the following: {Labels}.",
                    token.Line,
                    token.Column);
                _messages.Add(message);
            }
            else if (rawIndex.Length > 0)
            {
                var index = int.Parse(rawIndex);

                if (index < MinVarLabelIndex || index > MaxVarLabelIndex)
                {
                    var message = new Message(
                        SeverityEnum.Error,
                        $"Labels' indexes must go from {MinVarLabelIndex} to {MaxVarLabelIndex}, both included.",
                        token.Line,
                        token.Column);
                    _messages.Add(message);
                }
            }
        }

        void CheckValidVar(IToken token)
        {
            var text = token.Text;
            var letter = text[0];
            var rawIndex = text.Substring(1);

            if (letter == InputVar || letter == AuxVar)
            {
                if (rawIndex.Length > 0)
                {
                    var index = int.Parse(rawIndex);

                    if (index < MinVarLabelIndex || index > MaxVarLabelIndex)
                    {
                        var message = new Message(
                            SeverityEnum.Error,
                            $"Input and aux vars' indexes must go from {MinVarLabelIndex} to {MaxVarLabelIndex}, " +
                            "both included.",
                            token.Line,
                            token.Column);
                        _messages.Add(message);
                    }
                }
            }
            else // 'Y'
            {
                if (!rawIndex.Equals(string.Empty))
                {
                    var message = new Message(
                        SeverityEnum.Error,
                        $"There is only one output var '{OutputVar}', thus cannot have index.",
                        token.Line,
                        token.Column);
                    _messages.Add(message);
                }
            }
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
