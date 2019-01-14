namespace GoTo.Interpreter
{
    public struct Locals
    {
        public int[] X { get; set; }

        public int Y { get; set; }

        public int[] Z { get; set; }

        public override string ToString() => 
            $"X1={X[0]}, X2={X[1]}, X3={X[2]}, X4={X[3]}, X5={X[4]}, X6={X[5]}, X7={X[6]}, X8={X[7]}\n" +
            $"Y={Y}\n" +
            $"Z1={Z[0]}, Z2={Z[1]}, Z3={Z[2]}, Z4={Z[3]}, Z5={Z[4]}, Z6={Z[5]}, Z7={Z[6]}, Z8={Z[7]}";
    }
}