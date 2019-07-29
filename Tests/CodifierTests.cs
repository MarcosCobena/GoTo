using GoTo.Parser.AbstractSyntaxTree;
using Xunit;

public class CodifierTests
{
    [Theory]
    [InlineData("X", 2)]
    [InlineData("Y", 1)]
    [InlineData("Z", 3)]
    public void Variable(string rawVar, int result)
    {
        var var = new Var(rawVar);

        var number = CodifierHelpers.Codify(var);

        Assert.Equal(number, result);
    }

    [Theory]
    [InlineData("A1", 1)]
    [InlineData("B1", 2)]
    [InlineData("C1", 3)]
    [InlineData("D1", 4)]
    [InlineData("E", 5)]
    public void Label(string rawLabel, int result)
    {
        var label = new Label(rawLabel);

        var number = CodifierHelpers.Codify(label);

        Assert.Equal(number, result);
    }
}