using GoTo;
using Xunit;

namespace Tests
{
    public class AbstractSyntaxTreeTests
    {
        [Fact]
        public void SkipDifferentVars()
        {
            Compiler.Run("Y = Y + 1\nY = Y + 1");
        }
    }
}
