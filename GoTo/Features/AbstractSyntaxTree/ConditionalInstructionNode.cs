namespace GoTo.Features.AbstractSyntaxTree
{
    class ConditionalInstructionNode : InstructionNode
    {
        string _var;
        string _label;

        public ConditionalInstructionNode(string var, string label)
        {
            _var = var;
            _label = label;
        }
    }
}