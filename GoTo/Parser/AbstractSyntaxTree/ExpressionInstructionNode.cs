namespace GoTo.Parser.AbstractSyntaxTree
{
    abstract class ExpressionInstructionNode : InstructionNode
    {
        public ExpressionInstructionNode(string var, int line, int column = -1) : base(var, line, column)
        {
        }
    }
}