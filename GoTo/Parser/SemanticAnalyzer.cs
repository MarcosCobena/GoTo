using GoTo.Parser.AbstractSyntaxTree;
using GoTo.Parser;
using System.Collections.Generic;
using System.Linq;

namespace GoTo.Parser
{
     class SemanticAnalyzer
    {
        public static void CheckLastLineSkip(ProgramNode program, ref List<Message> messages)
        {
            var lastInstruction = program.Instructions.LastOrDefault();
            var outputVar = SemanticListener.OutputVar.ToString();

            if (lastInstruction is UnaryExpressionInstructionNode skipInstruction &&
                skipInstruction.VarType == ExpressionInstructionNode.VarTypeEnum.Output)
            {
                var message = new Message(
                    SeverityEnum.Error,
                    $"The last line cannot be a skip instruction with {outputVar} output var.",
                    lastInstruction.Line,
                    lastInstruction.Column);
                messages.Add(message);
            }
        }

        public static void CheckMissingLabel(ProgramNode program, ref List<Message> messages)
        {
            var conditionals = program.Instructions
                .Where(item => item is ConditionalInstructionNode node && 
                    !node.TargetLabel.Equals(Settings.ExitLabel.ToString()))
                .Cast<ConditionalInstructionNode>();
            var labels = program.Instructions
                .Cast<InstructionNode>()
                .Select(item => item.Label);

            foreach (var item in conditionals)
            {
                if (!labels.Contains(item.TargetLabel))
                {
                    var message = new Message(
                        SeverityEnum.Error,
                        $"The conditional instruction cannot target missing label {item.TargetLabel}.",
                        item.Line,
                        item.Column);
                    messages.Add(message);
                }
            }
        }
    }
}
