namespace GoTo.Parser.AbstractSyntaxTree
{
    public class ConditionalInstructionNode : InstructionNode
    {
        public ConditionalInstructionNode(string var, string label, int line, int column = -1) 
            : base(var, line, column)
        {
            TargetLabel = new Label(label);
        }

        public Label TargetLabel { get; }
    }
}