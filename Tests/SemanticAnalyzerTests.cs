using GoTo;
using System;
using System.Linq;
using Xunit;

namespace Tests
{
    public class SemanticAnalyzerTests
    {
        [Fact]
        public void SkipWithDifferentVars()
        {
            CheckDifferentVars("X = X2", "Skip");
        }

        [Fact]
        public void SkipWithEqualVars()
        {
            var messages = Compiler.Run("X = X");

            Assert.Empty(messages);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementWithDifferentVars(string @operator)
        {
            CheckDifferentVars($"X = X2 {@operator} 1", "Increment", "Decrement");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementWithEqualVars(string @operator)
        {
            var messages = Compiler.Run($"X = X {@operator} 1");

            Assert.Empty(messages);
        }

        [Fact]
        public void SkipWithInputVarIndexUpperLimit()
        {
            CheckInputVarIndex("X9 = X9");
        }

        [Fact]
        public void SkipWithInputVarLowerLimit()
        {
            CheckInputVarIndex("X0 = X0");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementInputVarIndexUpperLimit(string @operator)
        {
            CheckInputVarIndex($"X9 = X9 {@operator} 1");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementInputVarLowerLimit(string @operator)
        {
            CheckInputVarIndex($"X0 = X0 {@operator} 1");
        }

        [Fact]
        public void SkipWithAuxVarIndexUpperLimit()
        {
            CheckInputVarIndex("Z9 = Z9");
        }

        [Fact]
        public void SkipWithAuxVarLowerLimit()
        {
            CheckInputVarIndex("Z0 = Z0");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementAuxVarIndexUpperLimit(string @operator)
        {
            CheckInputVarIndex($"Z9 = Z9 {@operator} 1");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementAuxVarLowerLimit(string @operator)
        {
            CheckInputVarIndex($"Z0 = Z0 {@operator} 1");
        }

        [Fact]
        public void SkipWithOutputVarIndexUpperLimit()
        {
            CheckInputVarIndex("Y1 = Y1");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementOutputVarIndexUpperLimit(string @operator)
        {
            CheckInputVarIndex($"Y1 = Y1 {@operator} 1");
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

        static void CheckInputVarIndex(string input)
        {
            var messages = Compiler.Run(input);

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);

            Assert.Equal(2, errorMessages.Count());
            Assert.All(
                errorMessages,
                message => message.Description.Contains("index", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
