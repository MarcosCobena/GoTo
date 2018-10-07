using Antlr4.Runtime;
using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void Increment()
        {
            AssertParseTokensCountEquals("Y = Y + 1", 5);
        }

        [Fact]
        public void Conditional()
        {
            AssertParseTokensCountEquals("IF X != 0 GOTO A", 6);
        }

        [Fact]
        public void Labeled()
        {
            AssertParseTokensCountEquals("[A] X = X", 6);
        }

        [Fact]
        public void Multiline()
        {
            AssertParseTokensCountEquals(
                "X = X\n" +
                "X = X", 
                7);
        }

        [Fact]
        public void MultipleWhitespaces()
        {
            AssertParseTokensCountEquals(" X  =    X    ", 3);
        }

        [Fact]
        public void Tabs()
        {
            AssertParseTokensCountEquals("\t X = X", 3);
        }

        static void AssertParseTokensCountEquals(string program, int count)
        {
            var inputStream = CharStreams.fromstring(program);
            var lexer = new GoToLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GoToParser(tokenStream)
            {
                BuildParseTree = false
            };
            parser.program();

            var countWithEOF = count + 1;
            Assert.Equal(countWithEOF, tokenStream.Size);
        }
    }
}
