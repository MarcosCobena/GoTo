using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace GoTo.Features.Macros
{
    class MacroExpansionListener : GoToBaseListener
    {
        static readonly Dictionary<string, GoToParser.LineContext> _macros = 
            new Dictionary<string, GoToParser.LineContext>();
        private readonly CommonTokenStream _tokenStream;

        TokenStreamRewriter _rewrittenTokenStream;

        public MacroExpansionListener(CommonTokenStream tokenStream)
        {
            _tokenStream = tokenStream;
            _rewrittenTokenStream = new TokenStreamRewriter(tokenStream);
        }

        public TokenStreamRewriter RewrittenTokenStream => _rewrittenTokenStream;

        public override void ExitMacrodefinition([NotNull] GoToParser.MacrodefinitionContext context)
        {
            base.ExitMacrodefinition(context);

            var name = context.macrosignature().macro().macroname().GetText();
            var @params = context.macrosignature().macro().@params;
            var body = context.macrobody;
            _macros.Add(name, body);

            //context.children.Clear();
            _rewrittenTokenStream.Delete(context.Start, context.Stop);
        }

        public override void ExitMacroInstruction([NotNull] GoToParser.MacroInstructionContext context)
        {
            base.ExitMacroInstruction(context);

            var name = context.macro().macroname().GetText();

            if (_macros.TryGetValue(name, out GoToParser.LineContext body))
            {
                //context.RemoveLastChild();
                // I don't actually like this because lines, columns, etc. keep relevant to their original position
                //context.AddChild(body);
                var macro = _tokenStream.GetText(body.Start, body.Stop);
                _rewrittenTokenStream.Replace(context.Start, context.Stop, macro);
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
