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
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class GoToLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		LABELID=10, VARID=11, DIGIT=12, NEWLINE=13, EQUAL=14;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"LABELID", "VARID", "DIGIT", "NEWLINE", "EQUAL"
	};


	public GoToLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public GoToLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'['", "']'", "'IF'", "'!='", "'0'", "'GOTO'", "'+'", "'-'", "'1'", 
		null, null, null, null, "'='"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, "LABELID", 
		"VARID", "DIGIT", "NEWLINE", "EQUAL"
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

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static GoToLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x10', '\x43', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', 
		'\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\b', 
		'\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', '\x3', 
		'\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', 
		'\x3', '\xE', '\x6', '\xE', '>', '\n', '\xE', '\r', '\xE', '\xE', '\xE', 
		'?', '\x3', '\xF', '\x3', '\xF', '\x2', '\x2', '\x10', '\x3', '\x3', '\x5', 
		'\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', '\xF', '\t', 
		'\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', '\xE', 
		'\x1B', '\xF', '\x1D', '\x10', '\x3', '\x2', '\x6', '\x3', '\x2', '\x43', 
		'G', '\x3', '\x2', 'Z', '\\', '\x3', '\x2', '\x32', ';', '\x4', '\x2', 
		'\f', '\f', '\xF', '\xF', '\x2', '\x43', '\x2', '\x3', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x11', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x13', '\x3', '\x2', '\x2', '\x2', '\x2', '\x15', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x17', '\x3', '\x2', '\x2', '\x2', '\x2', '\x19', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x1B', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x3', '\x1F', '\x3', '\x2', 
		'\x2', '\x2', '\x5', '!', '\x3', '\x2', '\x2', '\x2', '\a', '#', '\x3', 
		'\x2', '\x2', '\x2', '\t', '&', '\x3', '\x2', '\x2', '\x2', '\v', ')', 
		'\x3', '\x2', '\x2', '\x2', '\r', '+', '\x3', '\x2', '\x2', '\x2', '\xF', 
		'\x30', '\x3', '\x2', '\x2', '\x2', '\x11', '\x32', '\x3', '\x2', '\x2', 
		'\x2', '\x13', '\x34', '\x3', '\x2', '\x2', '\x2', '\x15', '\x36', '\x3', 
		'\x2', '\x2', '\x2', '\x17', '\x38', '\x3', '\x2', '\x2', '\x2', '\x19', 
		':', '\x3', '\x2', '\x2', '\x2', '\x1B', '=', '\x3', '\x2', '\x2', '\x2', 
		'\x1D', '\x41', '\x3', '\x2', '\x2', '\x2', '\x1F', ' ', '\a', ']', '\x2', 
		'\x2', ' ', '\x4', '\x3', '\x2', '\x2', '\x2', '!', '\"', '\a', '_', '\x2', 
		'\x2', '\"', '\x6', '\x3', '\x2', '\x2', '\x2', '#', '$', '\a', 'K', '\x2', 
		'\x2', '$', '%', '\a', 'H', '\x2', '\x2', '%', '\b', '\x3', '\x2', '\x2', 
		'\x2', '&', '\'', '\a', '#', '\x2', '\x2', '\'', '(', '\a', '?', '\x2', 
		'\x2', '(', '\n', '\x3', '\x2', '\x2', '\x2', ')', '*', '\a', '\x32', 
		'\x2', '\x2', '*', '\f', '\x3', '\x2', '\x2', '\x2', '+', ',', '\a', 'I', 
		'\x2', '\x2', ',', '-', '\a', 'Q', '\x2', '\x2', '-', '.', '\a', 'V', 
		'\x2', '\x2', '.', '/', '\a', 'Q', '\x2', '\x2', '/', '\xE', '\x3', '\x2', 
		'\x2', '\x2', '\x30', '\x31', '\a', '-', '\x2', '\x2', '\x31', '\x10', 
		'\x3', '\x2', '\x2', '\x2', '\x32', '\x33', '\a', '/', '\x2', '\x2', '\x33', 
		'\x12', '\x3', '\x2', '\x2', '\x2', '\x34', '\x35', '\a', '\x33', '\x2', 
		'\x2', '\x35', '\x14', '\x3', '\x2', '\x2', '\x2', '\x36', '\x37', '\t', 
		'\x2', '\x2', '\x2', '\x37', '\x16', '\x3', '\x2', '\x2', '\x2', '\x38', 
		'\x39', '\t', '\x3', '\x2', '\x2', '\x39', '\x18', '\x3', '\x2', '\x2', 
		'\x2', ':', ';', '\t', '\x4', '\x2', '\x2', ';', '\x1A', '\x3', '\x2', 
		'\x2', '\x2', '<', '>', '\t', '\x5', '\x2', '\x2', '=', '<', '\x3', '\x2', 
		'\x2', '\x2', '>', '?', '\x3', '\x2', '\x2', '\x2', '?', '=', '\x3', '\x2', 
		'\x2', '\x2', '?', '@', '\x3', '\x2', '\x2', '\x2', '@', '\x1C', '\x3', 
		'\x2', '\x2', '\x2', '\x41', '\x42', '\a', '?', '\x2', '\x2', '\x42', 
		'\x1E', '\x3', '\x2', '\x2', '\x2', '\x4', '\x2', '?', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
