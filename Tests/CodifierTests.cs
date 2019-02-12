using GoTo.Parser.AbstractSyntaxTree;
using Xunit;

public class CodifierTests
{
    [Theory]
    [InlineData(InstructionNode.VarTypeEnum.Input, 1, 2)]
    [InlineData(InstructionNode.VarTypeEnum.Output, 1, 1)]
    [InlineData(InstructionNode.VarTypeEnum.Aux, 1, 3)]
    public void Var(InstructionNode.VarTypeEnum type, int index, int result)
    {
        var var = (type, index);

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