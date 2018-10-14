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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="GoToParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IGoToVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="GoToParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgram([NotNull] GoToParser.ProgramContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>UnlabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnlabeledLine([NotNull] GoToParser.UnlabeledLineContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>LabeledLine</c>
	/// labeled alternative in <see cref="GoToParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLabeledLine([NotNull] GoToParser.LabeledLineContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ExpressionInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpressionInstruction([NotNull] GoToParser.ExpressionInstructionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ConditionalInstruction</c>
	/// labeled alternative in <see cref="GoToParser.instruction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConditionalInstruction([NotNull] GoToParser.ConditionalInstructionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>BinaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBinaryExpression([NotNull] GoToParser.BinaryExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>UnaryExpression</c>
	/// labeled alternative in <see cref="GoToParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnaryExpression([NotNull] GoToParser.UnaryExpressionContext context);
}
