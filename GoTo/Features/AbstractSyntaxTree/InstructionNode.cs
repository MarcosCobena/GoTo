namespace GoTo.Features.AbstractSyntaxTree
{
    abstract class InstructionNode : GoToNode
    {
        public string Label { get; internal set; }
    }
}