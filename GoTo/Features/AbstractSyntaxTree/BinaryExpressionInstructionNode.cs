namespace GoTo.Features.AbstractSyntaxTree
{
    class BinaryExpressionInstructionNode : ExpressionInstructionNode
    {
        readonly OperatorEnum _operator;

        public enum OperatorEnum
        {
            Increment,
            Decrement
        }

        public BinaryExpressionInstructionNode(string var, string @operator) : base(var)
        {
            _operator = @operator == "+" ? 
                OperatorEnum.Increment :
                OperatorEnum.Decrement;
        }

        public OperatorEnum Operator => _operator;
    }
}