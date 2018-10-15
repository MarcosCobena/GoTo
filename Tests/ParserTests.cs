using GoTo;
using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void Multiline()
        {
            AssertEmptyMessages(
                "X = X\n" +
                "X2 = X2");
        }

        [Fact]
        public void MultipleWhitespaces()
        {
            AssertEmptyMessages(" X  =    X    ");
        }

        [Fact]
        public void Tabs()
        {
            AssertEmptyMessages("\t X = X");
        }

        static void AssertEmptyMessages(string input)
        {
            var messages = Compiler.Run(input);

            Assert.Empty(messages);
        }
    }
}
