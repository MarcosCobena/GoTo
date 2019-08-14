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

        public override string ToString() => 
            $"{base.ToString()}{Var} = {Var} " +
            $"{(_operator == OperatorEnum.Decrement ? "-" : "+")} 1";
    }
}