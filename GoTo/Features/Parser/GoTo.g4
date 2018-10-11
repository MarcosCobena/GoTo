grammar GoTo;

// rules must begin with lowercase, specially the first one
program : 
    line |
    (line NEWLINE)* ;

line : 
    instruction |
    '[' label ']' instruction ;

instruction : 
    var EQUAL expression |
    'IF' var '!=' '0' 'GOTO' label ;

expression : 
    var ('+' | '-')  '1' |
    var;

label : LABELID DIGIT* ;

var : VARID DIGIT* ;

LABELID : [A-E] ;

VARID : [XYZ] ;

DIGIT : [0-9] ;

NEWLINE : [\r\n]+ ;

EQUAL : '=' ;