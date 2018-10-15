using Antlr4.Runtime.Misc;

namespace GoTo.Features.AbstractSyntaxTree
{
    class AbstractSyntaxTreeGenerator : GoToBaseVisitor<GoToNode>
    {
        public override GoToNode VisitProgram([NotNull] GoToParser.ProgramContext context)
        {
            var programNode = new ProgramNode();

            var lines = context.line();

            foreach (var item in lines)
            {
                var instruction = Visit(item) as InstructionNode;
                programNode.Instructions.Add((null, instruction));
            }

            return programNode;
        }

        public override GoToNode VisitUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context) =>
            Visit(context.instruction());

        public override GoToNode VisitExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context)
            => Visit(context.expression());

        public override GoToNode VisitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context)
        {
            var instructionNode = new BinaryExpressionInstructionNode();

            return instructionNode;
        }
    }
}
