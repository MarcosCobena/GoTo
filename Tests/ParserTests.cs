using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void NoSpaces()
        {
            AssertExtensions.RunWithEmptyMessages("X=XX=X");
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
                "MACRO GOTO L\n" + // I want L to be understood as an input label
                "Z = Z + 1\n" + // Ídem for Z as a free aux var
                "IF Z != 0 GOTO L\n" +
                "END\n" +
                "GOTO E");
        }

        [Fact]
        public void ZeroMacro()
        {
            AssertExtensions.RunWithEmptyMessages(
                "MACRO ZERO V\n" + // I want V to be understood as an input var
                "[K] V = V - 1\n" + // Ídem for K as a free label
                "IF V != 0 GOTO K\n" +
                "END");
        }
    }
}
