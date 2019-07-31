using GoTo.Parser.AbstractSyntaxTree;
using Xunit;

public class CodifierTests
{
    [Theory]
    [InlineData("X", 2)]
    [InlineData("Y", 1)]
    [InlineData("Z", 3)]
    public void Variable(string rawVar, int expectedResult)
    {
        var var = new Var(rawVar);

        var number = CodifierHelpers.Codify(var);

        Assert.Equal(expectedResult, number);
    }

    [Theory]
    [InlineData("A1", 1)]
    [InlineData("B1", 2)]
    [InlineData("C1", 3)]
    [InlineData("D1", 4)]
    [InlineData("E", 5)]
    public void Label(string rawLabel, int expectedResult)
    {
        var label = new Label(rawLabel);

        var number = CodifierHelpers.Codify(label);

        Assert.Equal(expectedResult, number);
    }

    [Fact]
    public void UnaryExpressionInstructionFormat()
    {
        var instruction = new UnaryExpressionInstructionNode("X", 1);

        AssertEqualCodifiedFormat(instruction, 0);
    }

    [Fact]
    public void IncrementBinaryExpressionInstructionFormat()
    {
        var instruction = new BinaryExpressionInstructionNode("X", "+");

        AssertEqualCodifiedFormat(instruction, 1);
    }

     [Fact]
    public void DecrementBinaryExpressionInstructionFormat()
    {
        var instruction = new BinaryExpressionInstructionNode("X", "-");

        AssertEqualCodifiedFormat(instruction, 2);
    }

    [Fact]
    public void ConditionalInstructionFormat()
    {
        const string rawLabel = "A";

        var instruction = new ConditionalInstructionNode("X", rawLabel, 0);
        var label = new Label(rawLabel);
        var codifiedLabel = CodifierHelpers.Codify(label);

        AssertEqualCodifiedFormat(instruction, codifiedLabel + 2);
    }

    [Theory]
    [InlineData(1, 1, 5)]
    [InlineData(1, 5, 21)]
    public void Gödel(int a, int b, int expectedResult)
    {
        var number = CodifierHelpers.Gödel(a, b);

        Assert.Equal(expectedResult, number);
    }

    [Fact]
    public void Instruction()
    {
        var instruction = new BinaryExpressionInstructionNode("X", "+")
        {
            Label = new Label("A")
        };

        AssertEqualCodified(instruction, 21);
    }

    [Fact]
    public void Instruction2()
    {
        var instruction = new ConditionalInstructionNode("Y", "A", 0)
        {
            Label = new Label("A")
        };

        AssertEqualCodified(instruction, 29);
    }

    [Fact]
    public void Instruction3()
    {
        var instruction = new ConditionalInstructionNode("X", "A", 0);

        AssertEqualCodified(instruction, 46);
    }

    private void AssertEqualCodified(
        InstructionNode instruction,
        int expectedResult)
    {
        var number = CodifierHelpers.Codify(instruction);

        Assert.Equal(expectedResult, number);
    }

    private void AssertEqualCodifiedFormat(
        InstructionNode instruction,
        int expectedResult)
    {
        var number = CodifierHelpers.CodifyFormat(instruction);

        Assert.Equal(expectedResult, number);
    }
}