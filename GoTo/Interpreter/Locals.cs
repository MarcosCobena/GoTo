using System.Linq;
using System.Runtime.CompilerServices;

namespace GoTo.Interpreter
{
    public struct Locals
    {
        public int[] X { get; set; }

        public int Y { get; set; }

        public int[] Z { get; set; }

        public override string ToString() => 
            $"- {Dump(X, nameof(X))}\n" +
            $"- Y={Y}\n" +
            $"- {Dump(Z, nameof(Z))}";

        string Dump(int[] array, string member) =>
            string.Join(", ", array.Select((item, index) => $"{member}{index + 1}={item}"));
    }
}