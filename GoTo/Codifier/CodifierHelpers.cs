using System;
using GoTo.Parser.AbstractSyntaxTree;

// Based on (page 4): http://www.cs.us.es/cursos/mcc-2017/temas/tema-3-trans.pdf
public class CodifierHelpers
{
    public static int Codify((InstructionNode.VarTypeEnum type, int index) var)
    {
        int number;

        switch (var.type)
        {
            case InstructionNode.VarTypeEnum.Input:
                number = 2 * var.index;
                break;
            case InstructionNode.VarTypeEnum.Output:
                number = 1;
                break;
            case InstructionNode.VarTypeEnum.Aux:
                number = 2 * var.index + 1;
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
}