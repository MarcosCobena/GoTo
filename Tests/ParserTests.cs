using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void Multiline()
        {
            AssertExtensions.RunWithEmptyMessages(
                "X = X\n" +
                "X2 = X2");
        }

        [Fact]
        public void MultipleWhiteSpaces()
        {
            AssertExtensions.RunWithEmptyMessages(" X  =    X    ");
        }

        [Fact]
        public void Tabs()
        {
            AssertExtensions.RunWithEmptyMessages("\t X = X");
        }
    }
}
