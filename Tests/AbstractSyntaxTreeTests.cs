using GoTo;
using Xunit;

namespace Tests
{
    public class AbstractSyntaxTreeTests
    {
        [Fact]
        public void DeleteMe()
        {
            Compiler.Run(
                "[A] X = X - 1\n" +
                "Y = Y + 1\n" +
                "IF X != 0 GOTO A");
        }
    }
}
