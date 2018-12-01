using Xunit;

namespace Tests
{
    public class SemanticAnalyzerTests
    {
        #region Skip

        [Fact]
        public void DifferentVarsSkip()
        {
            AssertExtensions.SingleErrorContainingKeywords("X = X2", "skip");
        }

        [Fact]
        public void EqualVarsSkip()
        {
            AssertExtensions.AnalyzeWithEmptyMessages("X = X");
        }

        [Theory]
        [InlineData('X', 0)]
        [InlineData('X', 9)]
        [InlineData('Z', 0)]
        [InlineData('Z', 9)]
        public void InputAuxVarIndexSkip(char var, int index)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"{var}{index} = {var}{index}", "index");
        }

        [Fact]
        public void OutputVarIndexSkip()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword("Y1 = Y1", "index");
        }

        [Fact]
        public void OutputVarLastSkip()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword(
                "X = X + 1\n" +
                "Y = Y",
                "skip",
                1);
        }

        [Fact]
        public void LowercaseSkip()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword("x = x", "lowercase");
        }

        #endregion Skip

        #region IncrementOrDecrement

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void DifferentVarsIncrementOrDecrement(string @operator)
        {
            AssertExtensions.SingleErrorContainingKeywords($"X = X2 {@operator} 1", "increment", "decrement");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void EqualVarsIncrementOrDecrement(string @operator)
        {
            AssertExtensions.AnalyzeWithEmptyMessages($"X = X {@operator} 1");
        }

        [Theory]
        [InlineData('X', 0, "+")]
        [InlineData('X', 0, "-")]
        [InlineData('X', 9, "+")]
        [InlineData('X', 9, "-")]
        [InlineData('Z', 0, "+")]
        [InlineData('Z', 0, "-")]
        [InlineData('Z', 9, "+")]
        [InlineData('Z', 9, "-")]
        public void InputAuxVarIndexIncrementOrDecrement(char var, int index, string @operator)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"{var}{index} = {var}{index} {@operator} 1", "index");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void OutputVarIndexIncrementOrDecrement(string @operator)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"Y1 = Y1 {@operator} 1", "index");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void LowercaseIncrementOrDecrement(string @operator)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"x = x {@operator} 1", "lowercase");
        }

        #endregion IncrementOrDecrement

        #region Conditional

        [Theory]
        [InlineData('X', 0)]
        [InlineData('X', 9)]
        [InlineData('Z', 0)]
        [InlineData('Z', 9)]
        public void InputAuxVarIndexConditional(char var, int index)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"IF {var}{index} != 0 GOTO A", "index", expectedErrorsCount: 1);
        }

        [Fact]
        public void OutputVarIndexConditional()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"IF Y1 != 0 GOTO A", "index", expectedErrorsCount: 1);
        }

        #endregion Conditional

        #region Labels

        [Theory]
        [InlineData('A', 0)]
        [InlineData('A', 9)]
        [InlineData('B', 0)]
        [InlineData('B', 9)]
        [InlineData('C', 0)]
        [InlineData('C', 9)]
        [InlineData('D', 0)]
        [InlineData('D', 9)]
        [InlineData('F', 1)]
        public void LabeledLine(char label, int index)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"[{label}{index}] X = X", "label", expectedErrorsCount: 1);
        }

        [Theory]
        [InlineData('E')]
        [InlineData('E', 1)]
        public void ExitLabeledLine(char label, int? index = null)
        {
            var actualIndex = index.HasValue ? 
                index.Value.ToString() : 
                string.Empty;

            AssertExtensions.EqualErrorsCountContainingKeyword($"[{label}{actualIndex}] X = X", "exit", expectedErrorsCount: 1);
        }

        [Theory]
        [InlineData('A', 0)]
        [InlineData('A', 9)]
        [InlineData('B', 0)]
        [InlineData('B', 9)]
        [InlineData('C', 0)]
        [InlineData('C', 9)]
        [InlineData('D', 0)]
        [InlineData('D', 9)]
        [InlineData('E', 0)]
        [InlineData('E', 9)]
        [InlineData('F', 1)]
        public void ConditionalLabel(char label, int index)
        {
            AssertExtensions.EqualErrorsCountContainingKeyword($"IF X != 0 GOTO {label}{index}", "label", expectedErrorsCount: 1);
        }

        [Fact]
        public void LowercaseLabel()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword("[a] X = X", "lowercase", expectedErrorsCount: 1);
        }

        [Fact]
        public void LowercaseConditionalLabel()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword("IF X != 0 GOTO a", "lowercase", expectedErrorsCount: 1);
        }

        [Fact]
        public void MissingLabel()
        {
            AssertExtensions.EqualErrorsCountContainingKeyword("IF X != 0 GOTO A", "label", expectedErrorsCount: 1);
        }

        [Fact]
        public void LabelsUsedMoreThanOnce()
        {
            AssertExtensions.SingleErrorContainingKeywords(
                "[A] X = X " +
                "[A] X = X",
                "label", "one");
        }

        #endregion Labels

        #region Others

        [Fact]
        public void Empty()
        {
            AssertExtensions.AnalyzeWithAnyMessage(string.Empty);
        }

        [Fact]
        public void RandomInput()
        {
            AssertExtensions.SingleErrorContainingKeywords("Lorem ipsum", "unknown");
        }

        [Fact]
        public void InfiniteLoopSingleInstruction()
        {
            AssertExtensions.SingleErrorContainingKeywords("[A] IF X != 0 GOTO A", "infinite", "loop");
        }

        #endregion Others
    }
}
