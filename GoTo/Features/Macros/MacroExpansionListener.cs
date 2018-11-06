using Antlr4.Runtime.Misc;

namespace GoTo.Features.Macros
{
    class MacroExpansionListener : GoToBaseListener
    {
        public override void ExitMacroInstruction([NotNull] GoToParser.MacroInstructionContext context)
        {
            base.ExitMacroInstruction(context);
        }
    }
}
