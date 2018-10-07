using Antlr4.Runtime.Misc;

namespace GoTo.Features.SemanticAnalyzer
{
    public class SemanticListener : GoToBaseListener
    {
        public override void ExitExpression([NotNull] GoToParser.ExpressionContext context)
        {
            var leftSideVar = context.Parent.GetChild(0).GetText();
            var rightSideVar = context.GetChild(0).GetText();

            if (!leftSideVar.Equals(rightSideVar))
            {
                throw new ParseCanceledException("Skip instruction must have the same var at both sides.");
            }
        }
    }
}
