using GoTo;
using Xunit;

namespace Tests
{
    public class CompilerTests
    {
        [Fact]
        public void EmptyProgram()
        {
            AssertResultWhenNoInput(string.Empty, 0);
        }

        [Fact]
        public void OutputVarIncrement()
        {
            AssertResultWhenNoInput("Y = Y + 1", 1);
        }

        [Fact]
        public void OutputVarDecrement()
        {
            AssertResultWhenNoInput("Y = Y - 1", 0);
        }

        static void AssertResultWhenNoInput(string input, int expectedResult)
        {
            var (result, _) = Compiler.Run(input);

            Assert.Equal(expectedResult, result);
        }
    }
}
