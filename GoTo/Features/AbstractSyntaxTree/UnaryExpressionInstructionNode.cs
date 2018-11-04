namespace GoTo.Features.AbstractSyntaxTree
{
    class UnaryExpressionInstructionNode : ExpressionInstructionNode
    {
        public UnaryExpressionInstructionNode(string var, int line, int column = -1) : base(var, line, column)
        {
        }
    }
}