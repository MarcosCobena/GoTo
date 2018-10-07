using GoTo;
using Xunit;

namespace Tests
{
    public class CompilerTests
    {
        [Fact]
        public void Playground()
        {
            Compiler.Run("X = X2");
        }
    }
}
