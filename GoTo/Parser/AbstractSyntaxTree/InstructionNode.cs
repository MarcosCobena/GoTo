namespace GoTo.Parser.AbstractSyntaxTree
{
    public abstract class InstructionNode : GoToNode
    {
        readonly Var _var;
        readonly int _line;
        readonly int _column;

        protected InstructionNode(string var, int line, int column = -1)
        {
            _var = new Var(var);
            _line = line;
            _column = column;
        }

        public int Line => _line;

        public int Column => _column;

        public Label Label { get; set; }
        
        public Var Var => _var;

        public override string ToString() => 
            Label == null ? 
                string.Empty : 
                $"[{Label}] ";
    }
}