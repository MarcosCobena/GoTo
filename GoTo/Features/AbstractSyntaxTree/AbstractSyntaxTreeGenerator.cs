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
                programNode.Instructions.Add(instruction);
            }

            return programNode;
        }

        public override GoToNode VisitUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context) =>
            Visit(context.instruction());

        public override GoToNode VisitLabeledLine([NotNull] GoToParser.LabeledLineContext context)
        {
            var instruction = Visit(context.instruction()) as InstructionNode;
            instruction.Label = context.label.Text;

            return instruction;
        }

        public override GoToNode VisitExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context)
            => Visit(context.expression());

        public override GoToNode VisitConditionalInstruction(
            [NotNull] GoToParser.ConditionalInstructionContext context) => 
            new ConditionalInstructionNode(context.var.Text, context.label.Text);

        public override GoToNode VisitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context) => 
            new BinaryExpressionInstructionNode(context.var.Text, context.@operator.Text);

        public override GoToNode VisitUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context) => 
            new UnaryExpressionInstructionNode(context.var.Text);
    }
}
