using System;

namespace GoTo.Features.AbstractSyntaxTree
{
    abstract class InstructionNode : GoToNode
    {
        public enum VarTypeEnum
        {
            Input,
            Output,
            Aux
        }

        protected InstructionNode(string var)
        {
            var letter = var[0];

            switch (letter)
            {
                case 'X':
                    VarType = VarTypeEnum.Input;
                    break;
                case 'Y':
                    VarType = VarTypeEnum.Output;
                    break;
                case 'Z':
                    VarType = VarTypeEnum.Aux;
                    break;
                default:
                    throw new ArgumentException($"Unrecognized var type: {letter}", nameof(var));
            }

            if (var.Length > 1)
            {
                VarIndex = int.Parse(var.Substring(1));
            }
            else
            {
                VarIndex = 1;
            }
        }

        public string Label { get; internal set; }

        public VarTypeEnum VarType { get; }

        public int VarIndex { get; }
    }
}