namespace GoTo
{
    public partial class Message
    {
        public Message(SeverityEnum severity, string description, int line, int column)
        {
            Severity = severity;
            Description = description;
            Line = line;
            Column = column;
        }

        public SeverityEnum Severity { get; }

        public string Description { get; }

        public int Line { get; }

        public int Column { get; }
    }
}
