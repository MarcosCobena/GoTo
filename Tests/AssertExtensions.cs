using GoTo;
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
            var messages = Analyze(input);

            Assert.NotEmpty(messages);
        }

        internal static void AnalyzeWithEmptyMessages(string input)
        {
            var messages = Analyze(input);

            Assert.Empty(messages);
        }

        internal static void AssertSingleErrorContainingKeywords(string input, params string[] errorKeywords)
        {
            Language.Analyze(input, out List<Message> messages);

            var errorMessages = messages.Where(message => message.Severity == SeverityEnum.Error);

            Assert.Single(errorMessages);

            var firstError = errorMessages.First();

            foreach (var item in errorKeywords)
            {
                Assert.Contains(item, firstError.Description, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        static List<Message> Analyze(string input)
        {
            Language.Analyze(input, out List<Message> messages);

            return messages;
        }
    }
}
