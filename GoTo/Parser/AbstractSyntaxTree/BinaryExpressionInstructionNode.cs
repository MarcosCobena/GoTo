namespace GoTo.Parser.AbstractSyntaxTree
{
    public class BinaryExpressionInstructionNode : ExpressionInstructionNode
    {
        readonly OperatorEnum _operator;

        public enum OperatorEnum
        {
            Increment,
            Decrement
        }

        public BinaryExpressionInstructionNode(string var, string @operator) 
            : base(var, -1, -1)
        {
            _operator = @operator == "+" ? 
                OperatorEnum.Increment :
                OperatorEnum.Decrement;
        }

        public OperatorEnum Operator => _operator;
    }
}