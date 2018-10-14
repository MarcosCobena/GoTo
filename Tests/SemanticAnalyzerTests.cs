using GoTo;
using System;
using System.Linq;
using Xunit;

namespace Tests
{
    public class SemanticAnalyzerTests
    {
        [Fact]
        public void SkipDifferentVars()
        {
            CheckDifferentVars("X = X2", "Skip");
        }

        [Fact]
        public void SkipEqualVars()
        {
            var messages = Compiler.Run("X = X");

            Assert.Empty(messages);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementDifferentVars(string @operator)
        {
            CheckDifferentVars($"X = X2 {@operator} 1", "Increment", "Decrement");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementEqualVars(string @operator)
        {
            var messages = Compiler.Run($"X = X {@operator} 1");

            Assert.Empty(messages);
        }

        [Theory]
        [InlineData('X', 0)]
        [InlineData('X', 9)]
        [InlineData('Z', 0)]
        [InlineData('Z', 9)]
        public void SkipInputAuxVarIndex(char var, int index)
        {
            CheckErrorsContainKeyword($"{var}{index} = {var}{index}", "index");
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
        public void IncrementOrDecrementInputAuxVarIndex(char var, int index, string @operator)
        {
            CheckErrorsContainKeyword($"{var}{index} = {var}{index} {@operator} 1", "index");
        }

        [Fact]
        public void SkipOutputVarIndex()
        {
            CheckErrorsContainKeyword("Y1 = Y1", "index");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementOutputVarIndex(string @operator)
        {
            CheckErrorsContainKeyword($"Y1 = Y1 {@operator} 1", "index");
        }

        [Theory]
        [InlineData('X', 0)]
        [InlineData('X', 9)]
        [InlineData('Z', 0)]
        [InlineData('Z', 9)]
        public void ConditionalInputAuxVarIndex(char var, int index)
        {
            CheckErrorsContainKeyword($"IF {var}{index} != 0 GOTO A", "index", expectedErrorsCount: 1);
        }

        [Fact]
        public void ConditionalOutputVarIndex()
        {
            CheckErrorsContainKeyword($"IF Y1 != 0 GOTO A", "index", expectedErrorsCount: 1);
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
        [InlineData('F', 1)]
        public void LabeledLine(char label, int index)
        {
            CheckErrorsContainKeyword($"[{label}{index}] X = X", "label", expectedErrorsCount: 1);
        }

        [Theory]
        [InlineData('E')]
        [InlineData('E', 1)]
        public void ExitLabeledLine(char label, int? index = null)
        {
            var actualIndex = index.HasValue ? 
                index.Value.ToString() : 
                string.Empty;

            CheckErrorsContainKeyword($"[{label}{actualIndex}] X = X", "exit", expectedErrorsCount: 1);
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
            CheckErrorsContainKeyword($"IF X != 0 GOTO {label}{index}", "label", expectedErrorsCount: 1);
        }

        [Fact]
        public void LastSkipOutputVar()
        {
            CheckErrorsContainKeyword(
                $"X = X + 1\n" +
                "Y = Y", 
                "skip", 
                1);
        }

        [Fact]
        public void LowercaseSkipVars()
        {
            CheckErrorsContainKeyword("x = x", "lowercase");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void LowercaseIncrementOrDecrementVars(string @operator)
        {
            CheckErrorsContainKeyword($"x = x {@operator} 1", "lowercase");
        }

        [Fact]
        public void LowercaseLabel()
        {
            CheckErrorsContainKeyword("[a] X = X", "lowercase", expectedErrorsCount: 1);
        }

        [Fact]
        public void LowercaseConditionalLabel()
        {
            CheckErrorsContainKeyword("IF X != 0 GOTO a", "lowercase", expectedErrorsCount: 1);
        }

        static void CheckDifferentVars(string input, params string[] errorKeywords)
        {
            var messages = Compiler.Run(input);

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);
            var firstError = errorMessages.First();

            Assert.Single(errorMessages);

            foreach (var item in errorKeywords)
            {
                Assert.Contains(item, firstError.Description, StringComparison.InvariantCultureIgnoreCase);
            }
        }
        
        static void CheckErrorsContainKeyword(string input, string keyword, int expectedErrorsCount = 2)
        {
            var messages = Compiler.Run(input);

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);

            Assert.Equal(expectedErrorsCount, errorMessages.Count());
            Assert.All(
                errorMessages,
                message => Assert.Contains(keyword, message.Description, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
