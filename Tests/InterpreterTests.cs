using GoTo;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class InterpreterTests : AbstractRunTests
    {
        public InterpreterTests() : base(true)
        { }

        [Fact]
        public void InfiniteLoop()
        {
            Framework.TryRun(
                "[A] IF X != 0 GOTO A",
                out int _,
                out IEnumerable<Message> __,
                1,
                isInterpreted: true);
        }
    }
}
