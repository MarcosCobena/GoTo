grammar GoTo;

// rules must begin with lowercase, specially the first one
program : line
	| (line NEWLINE)*
	;

line : instruction #UnlabeledLine
	| '[' ID ']' instruction #LabeledLine
	;

instruction : ID '=' expression #ExpressionInstruction
	| 'IF' ID '!=' '0' 'GOTO' ID #ConditionalInstruction
	;

expression : ID ('+' | '-')  '1' # BinaryExpression
	| ID # UnaryExpression
	;

ID : LETTER DIGIT* ;

LETTER : [a-zA-Z] ;

DIGIT : [0-9] ;

NEWLINE : [\r\n]+ -> skip ;

WHITESPACE : [ \t]+ -> skip ;