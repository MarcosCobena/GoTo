using GoTo.Parser.AbstractSyntaxTree;
using Xunit;

namespace Tests
{
    public class LabelTests
    {
        [Fact]
        public void NullEquals()
        {
            Label label = null;

            Assert.True(label == null);
        }
    }
}
