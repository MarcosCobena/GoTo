namespace GoTo.Parser.AbstractSyntaxTree
{
    class ConditionalInstructionNode : InstructionNode
    {
        public ConditionalInstructionNode(string var, string label, int line, int column = -1) 
            : base(var, line, column)
        {
            TargetLabel = label;
        }

        public string TargetLabel { get; }
    }
}