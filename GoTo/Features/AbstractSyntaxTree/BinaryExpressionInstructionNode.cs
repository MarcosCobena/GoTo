namespace GoTo.Features.AbstractSyntaxTree
{
    class BinaryExpressionInstructionNode : ExpressionInstructionNode
    {
        readonly string _operator;

        public BinaryExpressionInstructionNode(string var, string @operator) : base(var)
        {
            _operator = @operator;
        }
    }
}