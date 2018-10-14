using System.Collections.Generic;

namespace GoTo.Features.AbstractSyntaxTree
{
    abstract class GoToNode
    { }

    class ProgramNode : GoToNode
    {
        public IEnumerable<(string label, InstructionNode instruction)> Instructions { get; set; }
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