using System.Collections.Generic;

namespace GoTo.Features.AbstractSyntaxTree
{
    class ProgramNode : GoToNode
    {
        readonly IList<InstructionNode> _instructions = new List<InstructionNode>();

        public IList<InstructionNode> Instructions => _instructions;
    }
}