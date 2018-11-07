using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace GoTo.Features.Macros
{
    class MacroExpansionListener : GoToBaseListener
    {
        static readonly Dictionary<string, GoToParser.LineContext> _macros = 
            new Dictionary<string, GoToParser.LineContext>();

        public override void ExitMacrodefinition([NotNull] GoToParser.MacrodefinitionContext context)
        {
            base.ExitMacrodefinition(context);

            var name = context.macrosignature().macro().macroname().GetText();
            var @params = context.macrosignature().macro().@params;
            var body = context.macrobody;
            _macros.Add(name, body);

            context.children.Clear();
        }

        public override void ExitMacroInstruction([NotNull] GoToParser.MacroInstructionContext context)
        {
            base.ExitMacroInstruction(context);

            var name = context.macro().macroname().GetText();

            if (_macros.TryGetValue(name, out GoToParser.LineContext body))
            {
                context.RemoveLastChild();
                // I don't actually like this because lines, columns, etc. keep relevant to their original position
                context.AddChild(body);
            }
        }

        public override void ExitProgram([NotNull] GoToParser.ProgramContext context)
        {
            base.ExitProgram(context);

            // EOF
            context.RemoveLastChild();
        }
    }
}
