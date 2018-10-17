using GoTo;
using Xunit;

namespace Tests
{
    public class CompilerTests
    {
        const string OutputPath = "Foo.dll";
        const string OutputType = "Foo";

        [Fact]
        public void DeleteMe()
        {
            var result = Compiler.Run(
                //"[A] X = X - 1\n" +
                //"Y = Y + 1\n" +
                //"IF X != 0 GOTO A",
                "Y = Y + 1",
                1);

            Assert.Equal(1, result);
        }
    }
}
