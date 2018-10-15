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

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class GoToParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, ID=11, LETTER=12, DIGIT=13, NEWLINE=14, WHITESPACE=15;
	public const int
		RULE_program = 0, RULE_line = 1, RULE_instruction = 2, RULE_expression = 3;
	public static readonly string[] ruleNames = {
		"program", "line", "instruction", "expression"
	};

	private static readonly string[] _LiteralNames = {
		null, "'['", "']'", "'='", "'IF'", "'!='", "'0'", "'GOTO'", "'+'", "'-'", 
		"'1'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, "ID", 
		"LETTER", "DIGIT", "NEWLINE", "WHITESPACE"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "GoTo.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static GoToParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public GoToParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public GoToParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}
	public partial class ProgramContext : ParserRuleContext {
		public ITerminalNode Eof() { return GetToken(GoToParser.Eof, 0); }
		public LineContext[] line() {
			return GetRuleContexts<LineContext>();
		}
		public LineContext line(int i) {
			return GetRuleContext<LineContext>(i);
		}
		public ProgramContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_program; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterProgram(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitProgram(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitProgram(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ProgramContext program() {
		ProgramContext _localctx = new ProgramContext(Context, State);
		EnterRule(_localctx, 0, RULE_program);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 11;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__3) | (1L << ID))) != 0)) {
				{
				{
				State = 8; line();
				}
				}
				State = 13;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 14; Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LineContext : ParserRuleContext {
		public LineContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_line; } }
	 
		public LineContext() { }
		public virtual void CopyFrom(LineContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class UnlabeledLineContext : LineContext {
		public InstructionContext instruction() {
			return GetRuleContext<InstructionContext>(0);
		}
		public UnlabeledLineContext(LineContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterUnlabeledLine(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitUnlabeledLine(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitUnlabeledLine(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class LabeledLineContext : LineContext {
		public IToken label;
		public InstructionContext instruction() {
			return GetRuleContext<InstructionContext>(0);
		}
		public ITerminalNode ID() { return GetToken(GoToParser.ID, 0); }
		public LabeledLineContext(LineContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterLabeledLine(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitLabeledLine(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitLabeledLine(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public LineContext line() {
		LineContext _localctx = new LineContext(Context, State);
		EnterRule(_localctx, 2, RULE_line);
		try {
			State = 21;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__3:
			case ID:
				_localctx = new UnlabeledLineContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 16; instruction();
				}
				break;
			case T__0:
				_localctx = new LabeledLineContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 17; Match(T__0);
				State = 18; ((LabeledLineContext)_localctx).label = Match(ID);
				State = 19; Match(T__1);
				State = 20; instruction();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class InstructionContext : ParserRuleContext {
		public InstructionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_instruction; } }
	 
		public InstructionContext() { }
		public virtual void CopyFrom(InstructionContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ExpressionInstructionContext : InstructionContext {
		public ITerminalNode ID() { return GetToken(GoToParser.ID, 0); }
		public ExpressionContext expression() {
			return GetRuleContext<ExpressionContext>(0);
		}
		public ExpressionInstructionContext(InstructionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterExpressionInstruction(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitExpressionInstruction(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitExpressionInstruction(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class ConditionalInstructionContext : InstructionContext {
		public IToken var;
		public IToken label;
		public ITerminalNode[] ID() { return GetTokens(GoToParser.ID); }
		public ITerminalNode ID(int i) {
			return GetToken(GoToParser.ID, i);
		}
		public ConditionalInstructionContext(InstructionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterConditionalInstruction(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitConditionalInstruction(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitConditionalInstruction(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public InstructionContext instruction() {
		InstructionContext _localctx = new InstructionContext(Context, State);
		EnterRule(_localctx, 4, RULE_instruction);
		try {
			State = 32;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case ID:
				_localctx = new ExpressionInstructionContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 23; Match(ID);
				State = 24; Match(T__2);
				State = 25; expression();
				}
				break;
			case T__3:
				_localctx = new ConditionalInstructionContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 26; Match(T__3);
				State = 27; ((ConditionalInstructionContext)_localctx).var = Match(ID);
				State = 28; Match(T__4);
				State = 29; Match(T__5);
				State = 30; Match(T__6);
				State = 31; ((ConditionalInstructionContext)_localctx).label = Match(ID);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ExpressionContext : ParserRuleContext {
		public ExpressionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_expression; } }
	 
		public ExpressionContext() { }
		public virtual void CopyFrom(ExpressionContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class BinaryExpressionContext : ExpressionContext {
		public IToken var;
		public IToken @operator;
		public ITerminalNode ID() { return GetToken(GoToParser.ID, 0); }
		public BinaryExpressionContext(ExpressionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterBinaryExpression(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitBinaryExpression(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitBinaryExpression(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class UnaryExpressionContext : ExpressionContext {
		public IToken var;
		public ITerminalNode ID() { return GetToken(GoToParser.ID, 0); }
		public UnaryExpressionContext(ExpressionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterUnaryExpression(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitUnaryExpression(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitUnaryExpression(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ExpressionContext expression() {
		ExpressionContext _localctx = new ExpressionContext(Context, State);
		EnterRule(_localctx, 6, RULE_expression);
		int _la;
		try {
			State = 38;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,3,Context) ) {
			case 1:
				_localctx = new BinaryExpressionContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 34; ((BinaryExpressionContext)_localctx).var = Match(ID);
				State = 35;
				((BinaryExpressionContext)_localctx).@operator = TokenStream.LT(1);
				_la = TokenStream.LA(1);
				if ( !(_la==T__7 || _la==T__8) ) {
					((BinaryExpressionContext)_localctx).@operator = ErrorHandler.RecoverInline(this);
				}
				else {
					ErrorHandler.ReportMatch(this);
				    Consume();
				}
				State = 36; Match(T__9);
				}
				break;
			case 2:
				_localctx = new UnaryExpressionContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 37; ((UnaryExpressionContext)_localctx).var = Match(ID);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\x11', '+', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x3', 
		'\x2', '\a', '\x2', '\f', '\n', '\x2', '\f', '\x2', '\xE', '\x2', '\xF', 
		'\v', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x5', '\x3', '\x18', '\n', '\x3', 
		'\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x5', '\x4', 
		'#', '\n', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', 
		'\x5', '\x5', ')', '\n', '\x5', '\x3', '\x5', '\x2', '\x2', '\x6', '\x2', 
		'\x4', '\x6', '\b', '\x2', '\x3', '\x3', '\x2', '\n', '\v', '\x2', '*', 
		'\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x4', '\x17', '\x3', '\x2', 
		'\x2', '\x2', '\x6', '\"', '\x3', '\x2', '\x2', '\x2', '\b', '(', '\x3', 
		'\x2', '\x2', '\x2', '\n', '\f', '\x5', '\x4', '\x3', '\x2', '\v', '\n', 
		'\x3', '\x2', '\x2', '\x2', '\f', '\xF', '\x3', '\x2', '\x2', '\x2', '\r', 
		'\v', '\x3', '\x2', '\x2', '\x2', '\r', '\xE', '\x3', '\x2', '\x2', '\x2', 
		'\xE', '\x10', '\x3', '\x2', '\x2', '\x2', '\xF', '\r', '\x3', '\x2', 
		'\x2', '\x2', '\x10', '\x11', '\a', '\x2', '\x2', '\x3', '\x11', '\x3', 
		'\x3', '\x2', '\x2', '\x2', '\x12', '\x18', '\x5', '\x6', '\x4', '\x2', 
		'\x13', '\x14', '\a', '\x3', '\x2', '\x2', '\x14', '\x15', '\a', '\r', 
		'\x2', '\x2', '\x15', '\x16', '\a', '\x4', '\x2', '\x2', '\x16', '\x18', 
		'\x5', '\x6', '\x4', '\x2', '\x17', '\x12', '\x3', '\x2', '\x2', '\x2', 
		'\x17', '\x13', '\x3', '\x2', '\x2', '\x2', '\x18', '\x5', '\x3', '\x2', 
		'\x2', '\x2', '\x19', '\x1A', '\a', '\r', '\x2', '\x2', '\x1A', '\x1B', 
		'\a', '\x5', '\x2', '\x2', '\x1B', '#', '\x5', '\b', '\x5', '\x2', '\x1C', 
		'\x1D', '\a', '\x6', '\x2', '\x2', '\x1D', '\x1E', '\a', '\r', '\x2', 
		'\x2', '\x1E', '\x1F', '\a', '\a', '\x2', '\x2', '\x1F', ' ', '\a', '\b', 
		'\x2', '\x2', ' ', '!', '\a', '\t', '\x2', '\x2', '!', '#', '\a', '\r', 
		'\x2', '\x2', '\"', '\x19', '\x3', '\x2', '\x2', '\x2', '\"', '\x1C', 
		'\x3', '\x2', '\x2', '\x2', '#', '\a', '\x3', '\x2', '\x2', '\x2', '$', 
		'%', '\a', '\r', '\x2', '\x2', '%', '&', '\t', '\x2', '\x2', '\x2', '&', 
		')', '\a', '\f', '\x2', '\x2', '\'', ')', '\a', '\r', '\x2', '\x2', '(', 
		'$', '\x3', '\x2', '\x2', '\x2', '(', '\'', '\x3', '\x2', '\x2', '\x2', 
		')', '\t', '\x3', '\x2', '\x2', '\x2', '\x6', '\r', '\x17', '\"', '(',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
