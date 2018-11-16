using GoTo;
using System.Collections.Generic;
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

        static List<Message> Analyze(string input)
        {
            Language.Analyze(input, out List<Message> messages);

            return messages;
        }
    }
}
