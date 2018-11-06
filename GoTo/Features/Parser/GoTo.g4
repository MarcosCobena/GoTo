grammar GoTo;

// rules must begin with lowercase, specially the first one
program : macrodefinition* line* EOF ;

macrodefinition : macrosignature line+ macroend
	;

macrosignature : 'MACRO' macro
	;

macro : macroname params=ID*
	;

macroname : WORD
	| 'GOTO'
	;

macroend : 'END'
	;

line : instruction #UnlabeledLine
	| '[' label=ID ']' instruction #LabeledLine
	;

instruction : ID '=' expression #ExpressionInstruction
	| 'IF' var=ID '!=' '0' 'GOTO' label=ID #ConditionalInstruction
	| macro #MacroInstruction
	;

expression : var=ID operator=('+' | '-')  '1' # BinaryExpression
	| var=ID # UnaryExpression
	;

ID : LETTER DIGIT* ;

LETTER : [a-zA-Z] ;

WORD : LETTER+ ;

DIGIT : [0-9] ;

NEWLINE : [\r\n]+ -> skip ;

WHITESPACE : [ \t]+ -> skip ;