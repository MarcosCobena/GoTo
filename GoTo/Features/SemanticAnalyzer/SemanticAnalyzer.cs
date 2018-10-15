using GoTo.Features.AbstractSyntaxTree;
using System.Collections.Generic;
using System.Linq;

namespace GoTo.Features.SemanticAnalyzer
{
     class SemanticAnalyzer
    {
        public static void CheckLastLineSkip(ProgramNode program, ref List<Message> messages)
        {
            var lastInstruction = program.Instructions.LastOrDefault();
            var outputVar = SemanticListener.OutputVar.ToString();

            if (lastInstruction is UnaryExpressionInstructionNode skipInstruction &&
                skipInstruction.Var == outputVar)
            {
                // TODO
                var message = new Message(
                    SeverityEnum.Error,
                    $"The last line cannot be a skip instruction with {outputVar} output var.",
                    -1,
                    -1);
                messages.Add(message);
            }
        }
    }
}
