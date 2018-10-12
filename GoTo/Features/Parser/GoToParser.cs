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
		LABEL=10, VAR=11, LABELID=12, VARID=13, DIGIT=14, NEWLINE=15, EQUAL=16;
	public const int
		RULE_program = 0, RULE_line = 1, RULE_instruction = 2, RULE_expression = 3;
	public static readonly string[] ruleNames = {
		"program", "line", "instruction", "expression"
	};

	private static readonly string[] _LiteralNames = {
		null, "'['", "']'", "'IF'", "'!='", "'0'", "'GOTO'", "'+'", "'-'", "'1'", 
		null, null, null, null, null, null, "'='"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, "LABEL", "VAR", 
		"LABELID", "VARID", "DIGIT", "NEWLINE", "EQUAL"
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
		public LineContext[] line() {
			return GetRuleContexts<LineContext>();
		}
		public LineContext line(int i) {
			return GetRuleContext<LineContext>(i);
		}
		public ITerminalNode[] NEWLINE() { return GetTokens(GoToParser.NEWLINE); }
		public ITerminalNode NEWLINE(int i) {
			return GetToken(GoToParser.NEWLINE, i);
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
	}

	[RuleVersion(0)]
	public ProgramContext program() {
		ProgramContext _localctx = new ProgramContext(Context, State);
		EnterRule(_localctx, 0, RULE_program);
		int _la;
		try {
			State = 17;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,1,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 8; line();
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 14;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__2) | (1L << VAR))) != 0)) {
					{
					{
					State = 9; line();
					State = 10; Match(NEWLINE);
					}
					}
					State = 16;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
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

	public partial class LineContext : ParserRuleContext {
		public InstructionContext instruction() {
			return GetRuleContext<InstructionContext>(0);
		}
		public ITerminalNode LABEL() { return GetToken(GoToParser.LABEL, 0); }
		public LineContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_line; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterLine(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitLine(this);
		}
	}

	[RuleVersion(0)]
	public LineContext line() {
		LineContext _localctx = new LineContext(Context, State);
		EnterRule(_localctx, 2, RULE_line);
		try {
			State = 24;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__2:
			case VAR:
				EnterOuterAlt(_localctx, 1);
				{
				State = 19; instruction();
				}
				break;
			case T__0:
				EnterOuterAlt(_localctx, 2);
				{
				State = 20; Match(T__0);
				State = 21; Match(LABEL);
				State = 22; Match(T__1);
				State = 23; instruction();
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
		public ITerminalNode VAR() { return GetToken(GoToParser.VAR, 0); }
		public ITerminalNode EQUAL() { return GetToken(GoToParser.EQUAL, 0); }
		public ExpressionContext expression() {
			return GetRuleContext<ExpressionContext>(0);
		}
		public ITerminalNode LABEL() { return GetToken(GoToParser.LABEL, 0); }
		public InstructionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_instruction; } }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterInstruction(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitInstruction(this);
		}
	}

	[RuleVersion(0)]
	public InstructionContext instruction() {
		InstructionContext _localctx = new InstructionContext(Context, State);
		EnterRule(_localctx, 4, RULE_instruction);
		try {
			State = 35;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case VAR:
				EnterOuterAlt(_localctx, 1);
				{
				State = 26; Match(VAR);
				State = 27; Match(EQUAL);
				State = 28; expression();
				}
				break;
			case T__2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 29; Match(T__2);
				State = 30; Match(VAR);
				State = 31; Match(T__3);
				State = 32; Match(T__4);
				State = 33; Match(T__5);
				State = 34; Match(LABEL);
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
		public ITerminalNode VAR() { return GetToken(GoToParser.VAR, 0); }
		public BinaryExpressionContext(ExpressionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterBinaryExpression(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitBinaryExpression(this);
		}
	}
	public partial class UnaryExpressionContext : ExpressionContext {
		public ITerminalNode VAR() { return GetToken(GoToParser.VAR, 0); }
		public UnaryExpressionContext(ExpressionContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.EnterUnaryExpression(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IGoToListener typedListener = listener as IGoToListener;
			if (typedListener != null) typedListener.ExitUnaryExpression(this);
		}
	}

	[RuleVersion(0)]
	public ExpressionContext expression() {
		ExpressionContext _localctx = new ExpressionContext(Context, State);
		EnterRule(_localctx, 6, RULE_expression);
		int _la;
		try {
			State = 41;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,4,Context) ) {
			case 1:
				_localctx = new BinaryExpressionContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 37; Match(VAR);
				State = 38;
				_la = TokenStream.LA(1);
				if ( !(_la==T__6 || _la==T__7) ) {
				ErrorHandler.RecoverInline(this);
				}
				else {
					ErrorHandler.ReportMatch(this);
				    Consume();
				}
				State = 39; Match(T__8);
				}
				break;
			case 2:
				_localctx = new UnaryExpressionContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 40; Match(VAR);
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
		'\x5964', '\x3', '\x12', '.', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\a', '\x2', '\xF', '\n', 
		'\x2', '\f', '\x2', '\xE', '\x2', '\x12', '\v', '\x2', '\x5', '\x2', '\x14', 
		'\n', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x5', '\x3', '\x1B', '\n', '\x3', '\x3', '\x4', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x4', '\x5', '\x4', '&', '\n', '\x4', '\x3', '\x5', 
		'\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x5', '\x5', ',', '\n', '\x5', 
		'\x3', '\x5', '\x2', '\x2', '\x6', '\x2', '\x4', '\x6', '\b', '\x2', '\x3', 
		'\x3', '\x2', '\t', '\n', '\x2', '.', '\x2', '\x13', '\x3', '\x2', '\x2', 
		'\x2', '\x4', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x6', '%', '\x3', '\x2', 
		'\x2', '\x2', '\b', '+', '\x3', '\x2', '\x2', '\x2', '\n', '\x14', '\x5', 
		'\x4', '\x3', '\x2', '\v', '\f', '\x5', '\x4', '\x3', '\x2', '\f', '\r', 
		'\a', '\x11', '\x2', '\x2', '\r', '\xF', '\x3', '\x2', '\x2', '\x2', '\xE', 
		'\v', '\x3', '\x2', '\x2', '\x2', '\xF', '\x12', '\x3', '\x2', '\x2', 
		'\x2', '\x10', '\xE', '\x3', '\x2', '\x2', '\x2', '\x10', '\x11', '\x3', 
		'\x2', '\x2', '\x2', '\x11', '\x14', '\x3', '\x2', '\x2', '\x2', '\x12', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x13', '\n', '\x3', '\x2', '\x2', 
		'\x2', '\x13', '\x10', '\x3', '\x2', '\x2', '\x2', '\x14', '\x3', '\x3', 
		'\x2', '\x2', '\x2', '\x15', '\x1B', '\x5', '\x6', '\x4', '\x2', '\x16', 
		'\x17', '\a', '\x3', '\x2', '\x2', '\x17', '\x18', '\a', '\f', '\x2', 
		'\x2', '\x18', '\x19', '\a', '\x4', '\x2', '\x2', '\x19', '\x1B', '\x5', 
		'\x6', '\x4', '\x2', '\x1A', '\x15', '\x3', '\x2', '\x2', '\x2', '\x1A', 
		'\x16', '\x3', '\x2', '\x2', '\x2', '\x1B', '\x5', '\x3', '\x2', '\x2', 
		'\x2', '\x1C', '\x1D', '\a', '\r', '\x2', '\x2', '\x1D', '\x1E', '\a', 
		'\x12', '\x2', '\x2', '\x1E', '&', '\x5', '\b', '\x5', '\x2', '\x1F', 
		' ', '\a', '\x5', '\x2', '\x2', ' ', '!', '\a', '\r', '\x2', '\x2', '!', 
		'\"', '\a', '\x6', '\x2', '\x2', '\"', '#', '\a', '\a', '\x2', '\x2', 
		'#', '$', '\a', '\b', '\x2', '\x2', '$', '&', '\a', '\f', '\x2', '\x2', 
		'%', '\x1C', '\x3', '\x2', '\x2', '\x2', '%', '\x1F', '\x3', '\x2', '\x2', 
		'\x2', '&', '\a', '\x3', '\x2', '\x2', '\x2', '\'', '(', '\a', '\r', '\x2', 
		'\x2', '(', ')', '\t', '\x2', '\x2', '\x2', ')', ',', '\a', '\v', '\x2', 
		'\x2', '*', ',', '\a', '\r', '\x2', '\x2', '+', '\'', '\x3', '\x2', '\x2', 
		'\x2', '+', '*', '\x3', '\x2', '\x2', '\x2', ',', '\t', '\x3', '\x2', 
		'\x2', '\x2', '\a', '\x10', '\x13', '\x1A', '%', '+',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
