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
            var messages = Compiler.Run("X = X2");

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);

            Assert.Single(errorMessages);
            Assert.Contains("Skip", errorMessages.First().Description);
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
            var messages = Compiler.Run($"X = X2 {@operator} 1");

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);
            var firstError = errorMessages.First();

            Assert.Single(errorMessages);
            Assert.Contains("Increment", firstError.Description, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains("Decrement", firstError.Description, StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public void IncrementOrDecrementWithEqualVars(string @operator)
        {
            var messages = Compiler.Run($"X = X {@operator} 1");

            Assert.Empty(messages);
        }
    }
}
