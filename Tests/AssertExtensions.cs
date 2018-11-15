﻿using GoTo;
using Xunit;

namespace Tests
{
    static class AssertExtensions
    {
        internal static void RunWithAnyMessage(string input)
        {
            var (_, messages) = Language.Run(input);

            Assert.NotEmpty(messages);
        }

        internal static void RunWithEmptyMessages(string input)
        {
            var (_, messages) = Language.Run(input);

            Assert.Empty(messages);
        }
    }
}
