using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void NoSpaces()
        {
            AssertExtensions.RunWithAnyMessage("X=XX=X");
        }

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

        [Fact]
        public void GoToMacro()
        {
            AssertExtensions.RunWithEmptyMessages(
                "MACRO GOTO L\n" +
                "Z = Z + 1\n" +
                "IF Z != 0 GOTO L\n" +
                "END\n" +
                "GOTO E");
        }

        [Fact]
        public void ZeroMacro()
        {
            AssertExtensions.RunWithEmptyMessages(
                "MACRO ZERO K V\n" +
                "[K] V = V - 1\n" +
                "IF V != 0 GOTO K\n" +
                "END\n" +
                "X = X + 1\n" +
                "ZERO A X");
        }
    }
}
