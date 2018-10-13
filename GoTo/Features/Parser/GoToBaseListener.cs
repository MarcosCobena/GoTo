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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IGoToListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class GoToBaseListener : IGoToListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="GoToParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterProgram([NotNull] GoToParser.ProgramContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="GoToParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitProgram([NotNull] GoToParser.ProgramContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>UnlabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>UnlabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>LabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLabeledLine([NotNull] GoToParser.LabeledLineContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>LabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLabeledLine([NotNull] GoToParser.LabeledLineContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>ExpressionInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>ExpressionInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>ConditionalInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalInstruction([NotNull] GoToParser.ConditionalInstructionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>ConditionalInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalInstruction([NotNull] GoToParser.ConditionalInstructionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>BinaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>BinaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>UnaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>UnaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
