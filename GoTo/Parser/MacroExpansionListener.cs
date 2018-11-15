using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace GoTo.Parser
{
    class MacroExpansionListener : GoToBaseListener
    {
        readonly IList<Message> _messages;
        readonly Dictionary<string, GoToParser.MacroBodyContext> _macrosBodies = 
            new Dictionary<string, GoToParser.MacroBodyContext>();
        readonly Dictionary<string, GoToParser.MacroParamsContext> _macrosParams = 
            new Dictionary<string, GoToParser.MacroParamsContext>();
        readonly CommonTokenStream _tokenStream;

        TokenStreamRewriter _rewrittenTokenStream;

        public MacroExpansionListener(CommonTokenStream tokenStream)
        {
            _tokenStream = tokenStream;
            _rewrittenTokenStream = new TokenStreamRewriter(tokenStream);
            _messages = new List<Message>();
        }

        public IEnumerable<Message> Messages => _messages;

        public TokenStreamRewriter RewrittenTokenStream => _rewrittenTokenStream;

        public override void ExitMacroDefinition([NotNull] GoToParser.MacroDefinitionContext context)
        {
            base.ExitMacroDefinition(context);

            var macro = context.macro();
            var name = macro.macroName().GetText();
            var @params = context.macro().macroParams();
            _macrosParams.Add(name, @params);
            var body = context.macroBody();
            _macrosBodies.Add(name, body);

            _rewrittenTokenStream.Delete(context.Start, context.Stop);
        }

        public override void ExitMacroInstruction([NotNull] GoToParser.MacroInstructionContext context)
        {
            base.ExitMacroInstruction(context);

            var macroContext = context.macro();
            var name = macroContext.macroName().GetText();
            var @params = macroContext.macroParams();

            if (_macrosBodies.TryGetValue(name, out GoToParser.MacroBodyContext body))
            {
                var macroBody = _tokenStream.GetText(body.Start, body.Stop);
                var macroParams = _macrosParams[name];
                var macroReplaced = macroBody;

                for (var i = 0; i < macroParams.ChildCount; i++)
                {
                    var sourceParam = macroParams.GetChild(i);
                    var targetParam = @params.GetChild(i);
                    macroReplaced = macroReplaced.Replace(sourceParam.GetText(), targetParam.GetText());
                    _rewrittenTokenStream.Replace(context.Start, context.Stop, macroReplaced);
                }
            }
        }
    }
}
