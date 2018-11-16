using System.Collections.Generic;

namespace GoTo.Parser.AbstractSyntaxTree
{
    public class ProgramNode : GoToNode
    {
        readonly IList<InstructionNode> _instructions = new List<InstructionNode>();

        public IList<InstructionNode> Instructions => _instructions;
    }
}