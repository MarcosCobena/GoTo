grammar GoTo;

// rules must begin with lowercase, specially the first one
program : line* EOF ;

line : instruction #UnlabeledLine
	| '[' label=ID ']' instruction #LabeledLine
	;

instruction : ID '=' expression #ExpressionInstruction
	| 'IF' var=ID '!=' '0' 'GOTO' label=ID #ConditionalInstruction
	;

expression : var=ID operator=('+' | '-')  '1' # BinaryExpression
	| var=ID # UnaryExpression
	;

ID : LETTER DIGIT* ;

LETTER : [a-zA-Z] ;

DIGIT : [0-9] ;

NEWLINE : [\r\n]+ -> skip ;

WHITESPACE : [ \t]+ -> skip ;