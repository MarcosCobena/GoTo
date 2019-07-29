using System;
using GoTo.Parser.AbstractSyntaxTree;

// Based on (page 4): http://www.cs.us.es/cursos/mcc-2017/temas/tema-3-trans.pdf
public class CodifierHelpers
{
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
}