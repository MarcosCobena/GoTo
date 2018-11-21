using GoTo;
using Xunit;

namespace Tests
{
    public class ILEmitterTests
    {
        #region Skip

        [Fact]
        public void Skip()
        {
            AssertResultWhenNoInput("X = X", 0);
        }

        #endregion Skip

        #region In/decrement

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
        public void InputVarDecrement()
        {
            AssertResultWhenNoInput(
                "X = X - 1 " +
                "X = X + 1 " +
                "IF X != 0 GOTO A " +
                "Z = Z + 1 " +
                "IF Z != 0 GOTO E " +
                "[A] Y = Y + 1", 
                1);
        }

        [Fact]
        public void OutputVarDecrement()
        {
            AssertResultWhenNoInput("Y = Y - 1", 0);
        }

        [Fact]
        public void AuxVarDecrement()
        {
            AssertResultWhenNoInput(
                "Z = Z - 1 " +
                "Z = Z + 1 " +
                "IF Z != 0 GOTO A " +
                "Z2 = Z2 + 1 " +
                "IF Z2 != 0 GOTO E " +
                "[A] Y = Y + 1",
                1);
        }

        #endregion In/decrement

        #region Conditional

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

        #endregion Conditional

        #region Labels

        [Fact]
        public void SenselessLabel()
        {
            AssertResultWhenNoInput("[A] X = X + 1", 0);
        }

        [Fact]
        public void ExitLabel()
        {
            AssertResultWhenNoInput("IF X != 0 GOTO E", 0);
        }

        #endregion Labels

        #region Macros

        [Fact]
        public void GoToMacro()
        {
            AssertResultWhenNoInput(
                "MACRO GOTO L\n" +
                "Z = Z + 1\n" +
                "IF Z != 0 GOTO L\n" +
                "END\n" +
                "Y = Y + 1\n" +
                "GOTO E\n" +
                "Y = Y + 1",
                1);
        }

        [Fact]
        public void ZeroMacro()
        {
            AssertResultWhenInput(
                "MACRO ZERO K V\n" +
                "[K] V = V - 1\n" +
                "IF V != 0 GOTO K\n" +
                "END\n" +
                "ZERO A X",
                int.MaxValue,
                0);
        }

        #endregion Macros

        #region Others

        [Fact]
        public void Empty()
        {
            AssertResultWhenNoInput(string.Empty, 0);
        }

        [Fact]
        public void InfiniteLoop()
        {
            Assert.Throws<InfiniteLoopException>(() => Language.Run("[A] IF X != 0 GOTO A", 1));
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

        #endregion Others

        static void AssertResultWhenInput(string input, int x1, int expectedResult)
        {
            var (result, _) = Language.Run(input, x1);

            Assert.Equal(expectedResult, result);
        }

        static void AssertResultWhenNoInput(string input, int expectedResult)
        {
            AssertResultWhenInput(input, 0, expectedResult);
        }
    }
}
