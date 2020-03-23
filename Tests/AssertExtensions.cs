using GoTo;
using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    static class AssertExtensions
    {
        internal static void AnalyzeWithAnyMessage(string input)
        {
            var isSuccess = Framework.TryAnalyze(
                input, 
                out string _, 
                out ProgramNode _, 
                out IEnumerable<Message> messages);

            Assert.False(isSuccess);
            Assert.NotEmpty(messages);
        }

        internal static void AnalyzeWithEmptyMessages(string input)
        {
            var isSuccess = Framework.TryAnalyze(
                input, 
                out string _, 
                out ProgramNode _, 
                out IEnumerable<Message> messages);

            Assert.True(isSuccess);
            Assert.Empty(messages);
        }
        
        // TODO refactor below two
        internal static void EqualErrorsCountContainingKeyword(
            string input, 
            string keyword, 
            int expectedErrorsCount = 2)
        {
            var isSuccess = Framework.TryAnalyze(
                input, 
                out string _,
                out ProgramNode _, 
                out IEnumerable<Message> messages);
            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);

            Assert.False(isSuccess);
            Assert.Equal(expectedErrorsCount, errorMessages.Count());
            Assert.All(
                errorMessages,
                message => Assert.Contains(keyword, message.Description, StringComparison.InvariantCultureIgnoreCase));
        }

        internal static void SingleErrorContainingKeywords(string input, params string[] errorKeywords)
        {
            var isSuccess = Framework.TryAnalyze(
                input, 
                out string _, 
                out ProgramNode _, 
                out IEnumerable<Message> messages);
            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);
            var firstError = errorMessages.First();

            Assert.False(isSuccess);
            Assert.Single(errorMessages);
            Assert.All(
                errorKeywords, 
                keyword => 
                    Assert.Contains(keyword, firstError.Description, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
