grammar GoTo;

// rules must begin with lowercase, specially the first one
program : line
	| (line NEWLINE)*
	;

line : instruction #UnlabeledLine
	| '[' LABEL ']' instruction #LabeledLine
	;

instruction : VAR EQUAL expression #ExpressionInstruction
	| 'IF' VAR '!=' '0' 'GOTO' LABEL #ConditionalInstruction
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