namespace GoTo.Features.AbstractSyntaxTree
{
    abstract class ExpressionInstructionNode : InstructionNode
    {
        readonly string _var;

        public ExpressionInstructionNode(string var)
        {
            _var = var;
        }

        public string Var => _var;
    }
}