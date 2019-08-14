using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GoTo.Parser.AbstractSyntaxTree;

namespace GoTo.Codifier
{
    // Based on (page 4): http://www.cs.us.es/cursos/mcc-2017/temas/tema-3-trans.pdf
    public static class CodifierHelpers
    {
        static readonly int[] primes = 
        {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 
            71, 73, 79, 83, 89, 97
        };

        public static int Codify(Var var)
        {
            int number;

            switch (var.Type)
            {
                case Var.VarTypeEnum.Input:
                    number = 2 * var.Index;
                    break;
                case Var.VarTypeEnum.Output:
                    number = 1;
                    break;
                case Var.VarTypeEnum.Aux:
                    number = 2 * var.Index + 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return number;
        }

        public static Var UncodifyVar(double number)
        {
            string rawVar;
            int index;

            if (number == 1)
            {
                rawVar = "Y";
                index = 1;
            }
            else if ((number % 2) == 0)
            {
                rawVar = "X";
                index = (int)number / 2;
            }
            else if ((number % 2) == 1)
            {
                rawVar = "Z";
                index = ((int)number - 1) / 2;
            }
            else
            {
                throw new InvalidOperationException();
            }
            
            var rawIndex = index > 1 ? index.ToString() : string.Empty;
            var var = new Var($"{rawVar}{rawIndex}");
            
            return var;
        }

        public static int Codify(Label label)
        {
            var labelMultiplier = label.Index - 1;
            int offset;

            switch (label.Id)
            {
                case Label.LabelIdEnum.A:
                    offset = 1;
                    break;
                case Label.LabelIdEnum.B:
                    offset = 2;
                    break;
                case Label.LabelIdEnum.C:
                    offset = 3;
                    break;
                case Label.LabelIdEnum.D:
                    offset = 4;
                    break;
                case Label.LabelIdEnum.E:
                    labelMultiplier = label.Index;
                    offset = 0;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var number = 5 * labelMultiplier + offset;

            return number;
        }

        public static Label UncodifyLabel(double number)
        {
            string id;
            var offset = number % 5;
            var multiplier = (number - offset) / 5;
            var index = multiplier + 1;

            switch (offset)
            {
                case 1:
                    id = "A";
                    break;
                case 2:
                    id = "B";
                    break;
                case 3:
                    id = "C";
                    break;
                case 4:
                    id = "D";
                    break;
                case 0:
                    id = "E";
                    index = multiplier;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var rawIndex = index > 1 ? index.ToString() : string.Empty;
            var label = new Label($"{id}{rawIndex}");

            return label;
        }

        public static int CodifyFormat(InstructionNode instruction)
        {
            int number;

            if (instruction is UnaryExpressionInstructionNode)
            {
                number = 0;
            }
            else if (instruction is BinaryExpressionInstructionNode 
                binaryExpression)
            {
                number = binaryExpression.Operator == 
                    BinaryExpressionInstructionNode.OperatorEnum.Increment ? 
                    1 : 
                    2;
            }
            else if (instruction is ConditionalInstructionNode conditional)
            {
                number = Codify(conditional.TargetLabel) + 2;
            }
            else
            {
                throw new InvalidOperationException();
            }

            return number;
        }

        public static InstructionNode UncodifyInstructionFormat(
            double number,
            Var var)
        {
            if (number < 0)
            {
                throw new InvalidOperationException();
            }

            InstructionNode fakeInstruction;

            if (number == 0)
            {
                fakeInstruction = new UnaryExpressionInstructionNode(
                    var.ToString(), 
                    -1);
            }
            else if (number == 1 || number == 2)
            {
                fakeInstruction = new BinaryExpressionInstructionNode(
                    var.ToString(), 
                    number == 1 ? "+" : "-");
            }
            else
            {
                var codifiedLabel = number - 2;
                var label = UncodifyLabel(codifiedLabel);
                fakeInstruction = new ConditionalInstructionNode(
                    var.ToString(), 
                    label.ToString(), 
                    -1);
            }

            return fakeInstruction;
        }

        public static double Codify(InstructionNode instruction)
        {
            var a = instruction.Label == null ?
                0 :
                Codify(instruction.Label);
            var b = CodifyFormat(instruction);
            var c = Codify(instruction.Var) - 1;
            var number = Pairing.Pair(a, Pairing.Pair(b, c));

            return number;
        }

        public static InstructionNode UncodifyInstruction(double number)
        {
            var ab = Pairing.Unpair(number);
            var bc = Pairing.Unpair(ab.b);
            var var = UncodifyVar(bc.b + 1);
            var instruction = UncodifyInstructionFormat(bc.a, var);

            if (ab.a != 0)
            {
                instruction.Label = UncodifyLabel(ab.a);
            }

            return instruction;
        }

        public static BigInteger Codify(ProgramNode program)
        {
            var number = new BigInteger(1);

            for (int i = 0; i < program.Instructions.Count; i++)
            {
                var prime = primes[i];
                
                var instruction = program.Instructions[i];
                var codifiedInstruction = Codify(instruction);
                
                number *= BigInteger.Pow(prime, (int)codifiedInstruction);
            }

            number -= 1;

            return number;
        }

        public static string UncodifyProgram(BigInteger number)
        {
            var numberQuote = number + 1;
            
            if (numberQuote == 1)
            {
                var singleInstruction = UncodifyInstruction(1);

                return singleInstruction.ToString();
            }

            var instructions = new List<double>();

            for (int i = 0; i < primes.Length; i++)
            {
                var prime = primes[i];
                var times = 0;

                var result = BigInteger.DivRem(
                    numberQuote,
                    prime,
                    out BigInteger remainder);

                while (remainder == 0)
                {
                    times++;

                    result = BigInteger.DivRem(
                        result,
                        prime,
                        out BigInteger remainderLocal);
                    remainder = remainderLocal;
                }

                instructions.Add(times);

                if (times > 0)
                {
                    numberQuote /= BigInteger.Pow(prime, times);
                }
            }

            instructions.RemoveAll(item => item == 0);

#if DEBUG
            Console.WriteLine($"{instructions.Count()} instructions");
#endif

            var program = UncodifyInstructions(instructions);

            return program;
        }

        static string UncodifyInstructions(List<double> instructions) =>
            instructions
                .Select(item => UncodifyInstruction(item))
                .Aggregate(
                    string.Empty,
                    (current, next) => current == string.Empty ? 
                        next.ToString() : 
                        $"{current}\n{next}");
    }
}