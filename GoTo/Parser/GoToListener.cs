//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from GoTo.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="GoToParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IGoToListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] GoToParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] GoToParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.macroDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMacroDefinition([NotNull] GoToParser.MacroDefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.macroDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMacroDefinition([NotNull] GoToParser.MacroDefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.macro"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMacro([NotNull] GoToParser.MacroContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.macro"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMacro([NotNull] GoToParser.MacroContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.macroName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMacroName([NotNull] GoToParser.MacroNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.macroName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMacroName([NotNull] GoToParser.MacroNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.macroParams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMacroParams([NotNull] GoToParser.MacroParamsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.macroParams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMacroParams([NotNull] GoToParser.MacroParamsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.macroBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMacroBody([NotNull] GoToParser.MacroBodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.macroBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMacroBody([NotNull] GoToParser.MacroBodyContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>UnlabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>UnlabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>LabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabeledLine([NotNull] GoToParser.LabeledLineContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>LabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabeledLine([NotNull] GoToParser.LabeledLineContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ExpressionInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ExpressionInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ConditionalInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalInstruction([NotNull] GoToParser.ConditionalInstructionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ConditionalInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalInstruction([NotNull] GoToParser.ConditionalInstructionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>MacroInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMacroInstruction([NotNull] GoToParser.MacroInstructionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>MacroInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMacroInstruction([NotNull] GoToParser.MacroInstructionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>BinaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>BinaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>UnaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>UnaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context);
}
