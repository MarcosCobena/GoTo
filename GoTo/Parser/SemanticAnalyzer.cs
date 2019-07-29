using System.Collections.Generic;
using System.Linq;
using GoTo.Parser.AbstractSyntaxTree;

namespace GoTo.Parser
{
    static class SemanticAnalyzer
    {
        internal static void Check(ProgramNode program, ref List<Message> messages)
        {
            CheckUnknownInput(program, ref messages);
            CheckLastLineSkip(program, ref messages);
            CheckMissingLabel(program, ref messages);
            CheckInfiniteLoop(program, ref messages);
        }

        static void CheckInfiniteLoop(ProgramNode program, ref List<Message> messages)
        {
            var loopConditionals = program.Instructions.Where(instruction =>
                instruction is ConditionalInstructionNode node &&
                node.TargetLabel == node.Label);

            foreach (var item in loopConditionals)
            {
                var message = new Message(
                    SeverityEnum.Error,
                    SemanticListener.InfiniteLoopMessage,
                    item.Line,
                    item.Column);
                messages.Add(message);
            }
        }

        static void CheckLastLineSkip(ProgramNode program, ref List<Message> messages)
        {
            var lastInstruction = program.Instructions.LastOrDefault();
            var outputVar = SemanticListener.OutputVar.ToString();

            if (lastInstruction is UnaryExpressionInstructionNode skipInstruction &&
                skipInstruction.Var.Type == Var.VarTypeEnum.Output)
            {
                var message = new Message(
                    SeverityEnum.Error,
                    $"The last line cannot be a skip instruction with {outputVar} output var.",
                    lastInstruction.Line,
                    lastInstruction.Column);
                messages.Add(message);
            }
        }

        static void CheckMissingLabel(ProgramNode program, ref List<Message> messages)
        {
            var conditionals = program.Instructions
                .Where(item => item is ConditionalInstructionNode node && node.TargetLabel.Id != Settings.ExitLabelId)
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

        static void CheckUnknownInput(ProgramNode program, ref List<Message> messages)
        {
            if (program.Instructions.All(instruction => instruction == null))
            {
                var message = new Message(
                    SeverityEnum.Error,
                    $"Unknown input.",
                    0,
                    0);
                messages.Add(message);
            }
        }
    }
}
