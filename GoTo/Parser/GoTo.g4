grammar GoTo;

// rules must begin with lowercase, specially the first one
program : macroDefinition* line* EOF ;

macroDefinition : 'MACRO' macro macroBody 'END' ;

macro : macroName macroParams ;

macroName : WORD
	| 'GOTO'
	;

macroParams : ID* ;

macroBody : line+ ;

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

NEWLINE : [\r\n]+ -> channel(HIDDEN) ;

WHITESPACE : [ \t]+ -> channel(HIDDEN) ;

COMMENT : ';' .*? '\n' -> skip ;