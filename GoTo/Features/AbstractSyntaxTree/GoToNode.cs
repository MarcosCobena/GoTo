using System.Collections.Generic;

namespace GoTo.Features.AbstractSyntaxTree
{
    abstract class GoToNode
    { }

    class ProgramNode : GoToNode
    {
        readonly IList<(string label, InstructionNode instruction)> _instructions = 
            new List<(string label, InstructionNode instruction)>();

        public IList<(string label, InstructionNode instruction)> Instructions => _instructions;
    }

    abstract class InstructionNode : GoToNode
    { }

    abstract class ExpressionInstructionNode : InstructionNode
    { }

    class BinaryExpressionInstructionNode : ExpressionInstructionNode
    { }

    class UnaryExpressionInstructionNode : ExpressionInstructionNode
    { }

    class ConditionalInstructionNode : InstructionNode
    { }
}