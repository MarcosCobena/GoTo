grammar GoTo;

// rules must begin with lowercase, specially the first one
program : line
	| (line NEWLINE)*
	;

line : instruction
	| '[' LABEL ']' instruction
	;

instruction : VAR EQUAL expression
	| 'IF' VAR '!=' '0' 'GOTO' LABEL
	;

expression : VAR ('+' | '-')  '1' # BinaryExpression
	| VAR # UnaryExpression
	;

LABEL : LABELID DIGIT* ;

VAR : VARID DIGIT* ;

LABELID : [A-E] ;

VARID : [XYZ] ;

DIGIT : [0-9] ;

NEWLINE : [\r\n]+ ;

EQUAL : '=' ;