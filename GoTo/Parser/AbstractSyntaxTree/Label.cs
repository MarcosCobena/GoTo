using System;

namespace GoTo.Parser.AbstractSyntaxTree
{
    public class Label
    {
        readonly string rawLabel;

        public enum LabelIdEnum
        {
            A,
            B,
            C,
            D,
            E
        }

        public Label(string rawLabel)
        {
            this.rawLabel = rawLabel;

            Parse(rawLabel);
        }

        public LabelIdEnum Id { get; private set; }
        
        public int Index { get; private set; }

        public static bool operator== (Label obj1, Label obj2) => 
            (object.ReferenceEquals(obj1, null) && object.ReferenceEquals(obj2, null)) ||
            !object.ReferenceEquals(obj1, null) && obj1.Equals(obj2);

        public static bool operator!= (Label obj1, Label obj2) => !(obj1 == obj2);

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null) || GetType() != obj.GetType())
            {
                return false;
            }
            
            var secondLabel = (Label)obj;

            return Id == secondLabel.Id && Index == secondLabel.Index;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => rawLabel;

        void Parse(string rawLabel)
        {
            Id = (LabelIdEnum)Enum.Parse(typeof(LabelIdEnum), $"{rawLabel[0]}");
            Index = rawLabel.Length > 1 ? 
                int.Parse(rawLabel.Substring(1)) :
                1;
        }
    }
}