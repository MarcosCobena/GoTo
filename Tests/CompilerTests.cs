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
        public void Skip()
        {
            AssertResultWhenNoInput("X = X", 0);
        }

        [Fact]
        public void InputVarIncrement()
        {
            AssertResultWhenNoInput("X = X + 1", 0);
        }

        [Fact]
        public void OutputVarIncrement()
        {
            AssertResultWhenNoInput("Y = Y + 1", 1);
        }

        [Fact]
        public void OutputVarDecrement()
        {
            // I'm not 100% sure should stick to the original specs where vars belong to K
            AssertResultWhenNoInput("Y = Y - 1", 0);
        }

        [Fact]
        public void SenselessLabel()
        {
            AssertResultWhenNoInput("[A] X = X + 1", 0);
        }

        [Fact]
        public void MissingLabel()
        {
            AssertResultWhenNoInput("IF X != 0 GOTO A", 0);
        }

        [Fact]
        public void InfiniteLoop()
        {
            AssertResultWhenNoInput("[A] IF X != 0 GOTO A", 0);
        }

        [Fact]
        public void Conditional()
        {
            AssertResultWhenInput(
                "Y = Y + 1\n" +
                "IF Y != 0 GOTO A\n" +
                "Y = Y + 1\n" +
                "[A] X = X",
                0,
                1);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public void CopyXIntoY(int x, int expectedY)
        {
            AssertResultWhenInput(
                "[A] X = X - 1\n" +
                "Y = Y + 1\n" +
                "IF X != 0 GOTO A",
                x,
                expectedY);
        }

        static void AssertResultWhenInput(string input, int x1, int expectedResult)
        {
            var (result, _) = Compiler.Run(input, x1);

            Assert.Equal(expectedResult, result);
        }

        static void AssertResultWhenNoInput(string input, int expectedResult)
        {
            AssertResultWhenInput(input, 0, expectedResult);
        }
    }
}
