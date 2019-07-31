using System;
using System.Numerics;
using GoTo.Parser.AbstractSyntaxTree;

// Based on (page 4): http://www.cs.us.es/cursos/mcc-2017/temas/tema-3-trans.pdf
public class CodifierHelpers
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

    public static double Codify(InstructionNode instruction)
    {
        var a = instruction.Label != null ?
            Codify(instruction.Label) :
            0;
        var b = CodifyFormat(instruction);
        var c = Codify(instruction.Var) - 1;
        var number = Gödel(a, Gödel(b, c));

        return number;
    }

    public static BigInteger Codify(ProgramNode program)
    {
        var number = new BigInteger(0);

        for (int i = 0; i < program.Instructions.Count; i++)
        {
            var prime = primes[i];
            
            var instruction = program.Instructions[i];
            var codifiedInstruction = Codify(instruction);
            
            number += BigInteger.Pow(prime, (int)codifiedInstruction);
        }

        number -= 1;

        return number;
    }

    public static double Gödel(double a, double b) => 
        (Math.Pow(2, a) * ((2 * b) + 1)) - 1;
}