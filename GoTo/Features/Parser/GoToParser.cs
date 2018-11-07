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
		T__9=10, T__10=11, T__11=12, ID=13, LETTER=14, WORD=15, DIGIT=16, NEWLINE=17, 
		WHITESPACE=18;
	public const int
		RULE_program = 0, RULE_macrodefinition = 1, RULE_macrosignature = 2, RULE_macro = 3, 
		RULE_macroname = 4, RULE_macroend = 5, RULE_line = 6, RULE_instruction = 7, 
		RULE_expression = 8;
	public static readonly string[] ruleNames = {
		"program", "macrodefinition", "macrosignature", "macro", "macroname", 
		"macroend", "line", "instruction", "expression"
	};

	private static readonly string[] _LiteralNames = {
		null, "'MACRO'", "'GOTO'", "'END'", "'['", "']'", "'='", "'IF'", "'!='", 
		"'0'", "'+'", "'-'", "'1'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, "ID", "LETTER", "WORD", "DIGIT", "NEWLINE", "WHITESPACE"
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
		public MacrodefinitionContext[] macrodefinition() {
			return GetRuleContexts<MacrodefinitionContext>();
		}
		public MacrodefinitionContext macrodefinition(int i) {
			return GetRuleContext<MacrodefinitionContext>(i);
		}
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
			State = 21;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==T__0) {
				{
				{
				State = 18; macrodefinition();
				}
				}
				State = 23;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 27;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__1) | (1L << T__3) | (1L << T__6) | (1L << ID) | (1L << WORD))) != 0)) {
				{
				{
				State = 24; line();
				}
				}
				State = 29;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 30; Match(Eof);
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

	public partial class MacrodefinitionContext : ParserRuleContext {
		public LineContext macrobody;
		public MacrosignatureContext macrosignature() {
			return GetRuleContext<MacrosignatureContext>(0);
		}
		public MacroendContext macroend() {
			return GetRuleContext<MacroendContext>(0);
		}
		public LineContext[] line() {
			return GetRuleContexts<LineContext>();
		}
		public LineContext line(int i) {
			return GetRuleContext<LineContext>(i);
		}
		public MacrodefinitionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_macrodefinition; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterMacrodefinition(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitMacrodefinition(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMacrodefinition(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public MacrodefinitionContext macrodefinition() {
		MacrodefinitionContext _localctx = new MacrodefinitionContext(Context, State);
		EnterRule(_localctx, 2, RULE_macrodefinition);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 32; macrosignature();
			State = 34;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			do {
				{
				{
				State = 33; _localctx.macrobody = line();
				}
				}
				State = 36;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__1) | (1L << T__3) | (1L << T__6) | (1L << ID) | (1L << WORD))) != 0) );
			State = 38; macroend();
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

	public partial class MacrosignatureContext : ParserRuleContext {
		public MacroContext macro() {
			return GetRuleContext<MacroContext>(0);
		}
		public MacrosignatureContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_macrosignature; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterMacrosignature(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitMacrosignature(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMacrosignature(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public MacrosignatureContext macrosignature() {
		MacrosignatureContext _localctx = new MacrosignatureContext(Context, State);
		EnterRule(_localctx, 4, RULE_macrosignature);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 40; Match(T__0);
			State = 41; macro();
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

	public partial class MacroContext : ParserRuleContext {
		public IToken @params;
		public MacronameContext macroname() {
			return GetRuleContext<MacronameContext>(0);
		}
		public ITerminalNode[] ID() { return GetTokens(GoToParser.ID); }
		public ITerminalNode ID(int i) {
			return GetToken(GoToParser.ID, i);
		}
		public MacroContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_macro; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterMacro(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitMacro(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMacro(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public MacroContext macro() {
		MacroContext _localctx = new MacroContext(Context, State);
		EnterRule(_localctx, 6, RULE_macro);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 43; macroname();
			State = 47;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					State = 44; _localctx.@params = Match(ID);
					}
					} 
				}
				State = 49;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			}
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

	public partial class MacronameContext : ParserRuleContext {
		public ITerminalNode WORD() { return GetToken(GoToParser.WORD, 0); }
		public MacronameContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_macroname; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterMacroname(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitMacroname(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMacroname(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public MacronameContext macroname() {
		MacronameContext _localctx = new MacronameContext(Context, State);
		EnterRule(_localctx, 8, RULE_macroname);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 50;
			_la = TokenStream.LA(1);
			if ( !(_la==T__1 || _la==WORD) ) {
			ErrorHandler.RecoverInline(this);
			}
			else {
				ErrorHandler.ReportMatch(this);
			    Consume();
			}
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

	public partial class MacroendContext : ParserRuleContext {
		public MacroendContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_macroend; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterMacroend(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitMacroend(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMacroend(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public MacroendContext macroend() {
		MacroendContext _localctx = new MacroendContext(Context, State);
		EnterRule(_localctx, 10, RULE_macroend);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 52; Match(T__2);
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
		EnterRule(_localctx, 12, RULE_line);
		try {
			State = 59;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__1:
			case T__6:
			case ID:
			case WORD:
				_localctx = new UnlabeledLineContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 54; instruction();
				}
				break;
			case T__3:
				_localctx = new LabeledLineContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 55; Match(T__3);
				State = 56; ((LabeledLineContext)_localctx).label = Match(ID);
				State = 57; Match(T__4);
				State = 58; instruction();
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
	public partial class MacroInstructionContext : InstructionContext {
		public MacroContext macro() {
			return GetRuleContext<MacroContext>(0);
		}
		public MacroInstructionContext(InstructionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterMacroInstruction(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitMacroInstruction(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IGoToVisitor<TResult> typedVisitor = visitor as IGoToVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMacroInstruction(this);
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
		EnterRule(_localctx, 14, RULE_instruction);
		try {
			State = 71;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case ID:
				_localctx = new ExpressionInstructionContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 61; Match(ID);
				State = 62; Match(T__5);
				State = 63; expression();
				}
				break;
			case T__6:
				_localctx = new ConditionalInstructionContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 64; Match(T__6);
				State = 65; ((ConditionalInstructionContext)_localctx).var = Match(ID);
				State = 66; Match(T__7);
				State = 67; Match(T__8);
				State = 68; Match(T__1);
				State = 69; ((ConditionalInstructionContext)_localctx).label = Match(ID);
				}
				break;
			case T__1:
			case WORD:
				_localctx = new MacroInstructionContext(_localctx);
				EnterOuterAlt(_localctx, 3);
				{
				State = 70; macro();
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
		EnterRule(_localctx, 16, RULE_expression);
		int _la;
		try {
			State = 77;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,6,Context) ) {
			case 1:
				_localctx = new BinaryExpressionContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 73; ((BinaryExpressionContext)_localctx).var = Match(ID);
				State = 74;
				((BinaryExpressionContext)_localctx).@operator = TokenStream.LT(1);
				_la = TokenStream.LA(1);
				if ( !(_la==T__9 || _la==T__10) ) {
					((BinaryExpressionContext)_localctx).@operator = ErrorHandler.RecoverInline(this);
				}
				else {
					ErrorHandler.ReportMatch(this);
				    Consume();
				}
				State = 75; Match(T__11);
				}
				break;
			case 2:
				_localctx = new UnaryExpressionContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 76; ((UnaryExpressionContext)_localctx).var = Match(ID);
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
		'\x5964', '\x3', '\x14', 'R', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x4', 
		'\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', '\b', 
		'\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x3', '\x2', '\a', 
		'\x2', '\x16', '\n', '\x2', '\f', '\x2', '\xE', '\x2', '\x19', '\v', '\x2', 
		'\x3', '\x2', '\a', '\x2', '\x1C', '\n', '\x2', '\f', '\x2', '\xE', '\x2', 
		'\x1F', '\v', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', 
		'\x3', '\x6', '\x3', '%', '\n', '\x3', '\r', '\x3', '\xE', '\x3', '&', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', 
		'\x3', '\x5', '\x3', '\x5', '\a', '\x5', '\x30', '\n', '\x5', '\f', '\x5', 
		'\xE', '\x5', '\x33', '\v', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', 
		'\a', '\x3', '\a', '\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\b', 
		'\x3', '\b', '\x5', '\b', '>', '\n', '\b', '\x3', '\t', '\x3', '\t', '\x3', 
		'\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', 
		'\x3', '\t', '\x3', '\t', '\x5', '\t', 'J', '\n', '\t', '\x3', '\n', '\x3', 
		'\n', '\x3', '\n', '\x3', '\n', '\x5', '\n', 'P', '\n', '\n', '\x3', '\n', 
		'\x2', '\x2', '\v', '\x2', '\x4', '\x6', '\b', '\n', '\f', '\xE', '\x10', 
		'\x12', '\x2', '\x4', '\x4', '\x2', '\x4', '\x4', '\x11', '\x11', '\x3', 
		'\x2', '\f', '\r', '\x2', 'P', '\x2', '\x17', '\x3', '\x2', '\x2', '\x2', 
		'\x4', '\"', '\x3', '\x2', '\x2', '\x2', '\x6', '*', '\x3', '\x2', '\x2', 
		'\x2', '\b', '-', '\x3', '\x2', '\x2', '\x2', '\n', '\x34', '\x3', '\x2', 
		'\x2', '\x2', '\f', '\x36', '\x3', '\x2', '\x2', '\x2', '\xE', '=', '\x3', 
		'\x2', '\x2', '\x2', '\x10', 'I', '\x3', '\x2', '\x2', '\x2', '\x12', 
		'O', '\x3', '\x2', '\x2', '\x2', '\x14', '\x16', '\x5', '\x4', '\x3', 
		'\x2', '\x15', '\x14', '\x3', '\x2', '\x2', '\x2', '\x16', '\x19', '\x3', 
		'\x2', '\x2', '\x2', '\x17', '\x15', '\x3', '\x2', '\x2', '\x2', '\x17', 
		'\x18', '\x3', '\x2', '\x2', '\x2', '\x18', '\x1D', '\x3', '\x2', '\x2', 
		'\x2', '\x19', '\x17', '\x3', '\x2', '\x2', '\x2', '\x1A', '\x1C', '\x5', 
		'\xE', '\b', '\x2', '\x1B', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x1C', 
		'\x1F', '\x3', '\x2', '\x2', '\x2', '\x1D', '\x1B', '\x3', '\x2', '\x2', 
		'\x2', '\x1D', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x1E', ' ', '\x3', 
		'\x2', '\x2', '\x2', '\x1F', '\x1D', '\x3', '\x2', '\x2', '\x2', ' ', 
		'!', '\a', '\x2', '\x2', '\x3', '!', '\x3', '\x3', '\x2', '\x2', '\x2', 
		'\"', '$', '\x5', '\x6', '\x4', '\x2', '#', '%', '\x5', '\xE', '\b', '\x2', 
		'$', '#', '\x3', '\x2', '\x2', '\x2', '%', '&', '\x3', '\x2', '\x2', '\x2', 
		'&', '$', '\x3', '\x2', '\x2', '\x2', '&', '\'', '\x3', '\x2', '\x2', 
		'\x2', '\'', '(', '\x3', '\x2', '\x2', '\x2', '(', ')', '\x5', '\f', '\a', 
		'\x2', ')', '\x5', '\x3', '\x2', '\x2', '\x2', '*', '+', '\a', '\x3', 
		'\x2', '\x2', '+', ',', '\x5', '\b', '\x5', '\x2', ',', '\a', '\x3', '\x2', 
		'\x2', '\x2', '-', '\x31', '\x5', '\n', '\x6', '\x2', '.', '\x30', '\a', 
		'\xF', '\x2', '\x2', '/', '.', '\x3', '\x2', '\x2', '\x2', '\x30', '\x33', 
		'\x3', '\x2', '\x2', '\x2', '\x31', '/', '\x3', '\x2', '\x2', '\x2', '\x31', 
		'\x32', '\x3', '\x2', '\x2', '\x2', '\x32', '\t', '\x3', '\x2', '\x2', 
		'\x2', '\x33', '\x31', '\x3', '\x2', '\x2', '\x2', '\x34', '\x35', '\t', 
		'\x2', '\x2', '\x2', '\x35', '\v', '\x3', '\x2', '\x2', '\x2', '\x36', 
		'\x37', '\a', '\x5', '\x2', '\x2', '\x37', '\r', '\x3', '\x2', '\x2', 
		'\x2', '\x38', '>', '\x5', '\x10', '\t', '\x2', '\x39', ':', '\a', '\x6', 
		'\x2', '\x2', ':', ';', '\a', '\xF', '\x2', '\x2', ';', '<', '\a', '\a', 
		'\x2', '\x2', '<', '>', '\x5', '\x10', '\t', '\x2', '=', '\x38', '\x3', 
		'\x2', '\x2', '\x2', '=', '\x39', '\x3', '\x2', '\x2', '\x2', '>', '\xF', 
		'\x3', '\x2', '\x2', '\x2', '?', '@', '\a', '\xF', '\x2', '\x2', '@', 
		'\x41', '\a', '\b', '\x2', '\x2', '\x41', 'J', '\x5', '\x12', '\n', '\x2', 
		'\x42', '\x43', '\a', '\t', '\x2', '\x2', '\x43', '\x44', '\a', '\xF', 
		'\x2', '\x2', '\x44', '\x45', '\a', '\n', '\x2', '\x2', '\x45', '\x46', 
		'\a', '\v', '\x2', '\x2', '\x46', 'G', '\a', '\x4', '\x2', '\x2', 'G', 
		'J', '\a', '\xF', '\x2', '\x2', 'H', 'J', '\x5', '\b', '\x5', '\x2', 'I', 
		'?', '\x3', '\x2', '\x2', '\x2', 'I', '\x42', '\x3', '\x2', '\x2', '\x2', 
		'I', 'H', '\x3', '\x2', '\x2', '\x2', 'J', '\x11', '\x3', '\x2', '\x2', 
		'\x2', 'K', 'L', '\a', '\xF', '\x2', '\x2', 'L', 'M', '\t', '\x3', '\x2', 
		'\x2', 'M', 'P', '\a', '\xE', '\x2', '\x2', 'N', 'P', '\a', '\xF', '\x2', 
		'\x2', 'O', 'K', '\x3', '\x2', '\x2', '\x2', 'O', 'N', '\x3', '\x2', '\x2', 
		'\x2', 'P', '\x13', '\x3', '\x2', '\x2', '\x2', '\t', '\x17', '\x1D', 
		'&', '\x31', '=', 'I', 'O',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
