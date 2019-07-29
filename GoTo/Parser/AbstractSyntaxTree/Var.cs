using System;

namespace GoTo.Parser.AbstractSyntaxTree
{
    public class Var
    {
        readonly string rawVar;

        public enum VarTypeEnum
        {
            Input,
            Output,
            Aux
        }

        public Var(string rawVar)
        {
            this.rawVar = rawVar;

            Parse(rawVar);
        }

        public VarTypeEnum Type { get; private set; }
        
        public int Index { get; private set; }

        public static bool operator== (Var obj1, Var obj2) => 
            (object.ReferenceEquals(obj1, null) && object.ReferenceEquals(obj2, null)) ||
            !object.ReferenceEquals(obj1, null) && obj1.Equals(obj2);

        public static bool operator!= (Var obj1, Var obj2) => !(obj1 == obj2);

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null) || GetType() != obj.GetType())
            {
                return false;
            }
            
            var secondVar = (Var)obj;

            return Type == secondVar.Type && Index == secondVar.Index;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => rawVar;

        void Parse(string rawVar)
        {
            var letter = rawVar[0];

            switch (letter)
            {
                case 'X':
                    Type = VarTypeEnum.Input;
                    break;
                case 'Y':
                    Type = VarTypeEnum.Output;
                    break;
                case 'Z':
                    Type = VarTypeEnum.Aux;
                    break;
                default:
                    throw new ArgumentException(
                        $"Unrecognized var type: {letter}", nameof(rawVar));
            }

            if (rawVar.Length > 1)
            {
                Index = int.Parse(rawVar.Substring(1));
            }
            else
            {
                Index = 1;
            }
        }
    }
}