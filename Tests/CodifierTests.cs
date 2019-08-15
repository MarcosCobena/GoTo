using System.Collections.Generic;
using System.Numerics;
using GoTo;
using GoTo.Codifier;
using GoTo.Parser.AbstractSyntaxTree;
using Xunit;

namespace Tests
{
    public class CodifierTests
    {
        [Theory]
        [InlineData("X", 2)]
        [InlineData("Y", 1)]
        [InlineData("Z", 3)]
        public void Variable(string rawVar, int expectedResult)
        {
            var var = new Var(rawVar);

            var number = Codifier.Codify(var);

            Assert.Equal(expectedResult, number);

            var uncodifiedVar = Codifier.UncodifyVar(number);

            Assert.Equal(var, uncodifiedVar);
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

            var number = Codifier.Codify(label);

            Assert.Equal(expectedResult, number);

            var uncodifiedLabel = Codifier.UncodifyLabel(number);

            Assert.Equal(label, uncodifiedLabel);
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
            var codifiedLabel = Codifier.Codify(label);

            AssertEqualCodifiedFormat(instruction, codifiedLabel + 2);
        }

        [Theory]
        [InlineData(1, 1, 5)]
        [InlineData(1, 5, 21)]
        public void Pair(int a, int b, int expectedResult)
        {
            var c = PairingHelpers.Pair(a, b);

            Assert.Equal(expectedResult, c);

            var ab = PairingHelpers.Unpair(c);

            Assert.Equal(a, ab.a);
            Assert.Equal(b, ab.b);
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

        [Fact]
        public void Program()
        {
            AssertEqualCodified(
                "[A] X = X + 1 IF X != 0 GOTO A",
                "18586928403505481978329694207");
        }

        [Fact]
        public void Program2()
        {
            AssertEqualCodified(
                "Y = Y",
                "0");
        }

        // [Fact]
        // public void Foo()
        // {
        //     var number = BigInteger.Parse("1");
        //     var program = CodifierHelpers.UncodifyProgram(number);
        //     throw new System.Exception(program);
        // }

        private void AssertEqualCodified(string inputProgram, string expectedResult)
        {
            Framework.TryAnalyze(
                inputProgram,
                out ProgramNode program,
                out IEnumerable<Message> _);
            
            var number = Codifier.Codify(program);

            var expected = BigInteger.Parse(expectedResult);
            Assert.Equal(expected, number);

            if (expected == 0)
            {
                // The uncodified instruction [E] Y = Y may not equal passed one
                // but both are actually the same
                return;
            }

            var uncodifiedProgram = Codifier.UncodifyProgram(number);

            Assert.Equal(
                inputProgram, 
                uncodifiedProgram.Replace("\n", " "));
        }

        private void AssertEqualCodified(
            InstructionNode instruction,
            int expectedResult)
        {
            var number = Codifier.Codify(instruction);

            Assert.Equal(expectedResult, number);
            
            var uncodifiedInstruction = Codifier.UncodifyInstruction(number);

            Assert.Equal(instruction.ToString(), uncodifiedInstruction.ToString());
        }

        private void AssertEqualCodifiedFormat(
            InstructionNode instruction,
            int expectedResult)
        {
            var number = Codifier.CodifyFormat(instruction);

            Assert.Equal(expectedResult, number);
        }
    }
}