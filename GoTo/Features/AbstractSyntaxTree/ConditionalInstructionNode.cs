namespace GoTo.Features.AbstractSyntaxTree
{
    class ConditionalInstructionNode : InstructionNode
    {
        public ConditionalInstructionNode(string var, string label) : base(var)
        {
            TargetLabel = label;
        }

        public string TargetLabel { get; }
    }
}