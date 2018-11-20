using GoTo;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class SemanticAnalyzerTests
    {
        #region Skip

        [Fact]
        public void DifferentVarsSkip()
        {
            AssertExtensions.AssertSingleErrorContainingKeywords("X = X2", "skip");
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
            AssertEqualErrorsCountContainingKeyword($"{var}{index} = {var}{index}", "index");
        }

        [Fact]
        public void OutputVarIndexSkip()
        {
            AssertEqualErrorsCountContainingKeyword("Y1 = Y1", "index");
        }

        [Fact]
        public void OutputVarLastSkip()
        {
            AssertEqualErrorsCountContainingKeyword(
                "X = X + 1\n" +
                "Y = Y",
                "skip",
                1);
        }

        [Fact]
        public void LowercaseSkip()
        {
            AssertEqualErrorsCountContainingKeyword("x = x", "lowercase");
        }

        #endregion Skip

        #region IncrementOrDecrement

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void DifferentVarsIncrementOrDecrement(string @operator)
        {
            AssertExtensions.AssertSingleErrorContainingKeywords($"X = X2 {@operator} 1", "increment", "decrement");
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
            AssertEqualErrorsCountContainingKeyword($"{var}{index} = {var}{index} {@operator} 1", "index");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void OutputVarIndexIncrementOrDecrement(string @operator)
        {
            AssertEqualErrorsCountContainingKeyword($"Y1 = Y1 {@operator} 1", "index");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void LowercaseIncrementOrDecrement(string @operator)
        {
            AssertEqualErrorsCountContainingKeyword($"x = x {@operator} 1", "lowercase");
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
            AssertEqualErrorsCountContainingKeyword($"IF {var}{index} != 0 GOTO A", "index", expectedErrorsCount: 1);
        }

        [Fact]
        public void OutputVarIndexConditional()
        {
            AssertEqualErrorsCountContainingKeyword($"IF Y1 != 0 GOTO A", "index", expectedErrorsCount: 1);
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
            AssertEqualErrorsCountContainingKeyword($"[{label}{index}] X = X", "label", expectedErrorsCount: 1);
        }

        [Theory]
        [InlineData('E')]
        [InlineData('E', 1)]
        public void ExitLabeledLine(char label, int? index = null)
        {
            var actualIndex = index.HasValue ? 
                index.Value.ToString() : 
                string.Empty;

            AssertEqualErrorsCountContainingKeyword($"[{label}{actualIndex}] X = X", "exit", expectedErrorsCount: 1);
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
            AssertEqualErrorsCountContainingKeyword($"IF X != 0 GOTO {label}{index}", "label", expectedErrorsCount: 1);
        }

        [Fact]
        public void LowercaseLabel()
        {
            AssertEqualErrorsCountContainingKeyword("[a] X = X", "lowercase", expectedErrorsCount: 1);
        }

        [Fact]
        public void LowercaseConditionalLabel()
        {
            AssertEqualErrorsCountContainingKeyword("IF X != 0 GOTO a", "lowercase", expectedErrorsCount: 1);
        }

        [Fact]
        public void MissingLabel()
        {
            AssertEqualErrorsCountContainingKeyword("IF X != 0 GOTO A", "label", expectedErrorsCount: 1);
        }

        [Fact]
        public void LabelsUsedMoreThanOnce()
        {
            AssertExtensions.AssertSingleErrorContainingKeywords(
                "[A] X = X " +
                "[A] X = X",
                "label", "one");
        }

        #endregion Labels

        #region Others

        [Fact]
        public void RandomInput()
        {
            AssertExtensions.AssertSingleErrorContainingKeywords("Lorem ipsum", "unknown");
        }

        #endregion Others

        static void AssertEqualErrorsCountContainingKeyword(string input, string keyword, int expectedErrorsCount = 2)
        {
            Language.Analyze(input, out List<Message> messages);

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);

            Assert.Equal(expectedErrorsCount, errorMessages.Count());
            Assert.All(
                errorMessages,
                message => Assert.Contains(keyword, message.Description, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
