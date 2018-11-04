using System;

namespace GoTo.Features.AbstractSyntaxTree
{
    abstract class InstructionNode : GoToNode
    {
        readonly int _line;
        readonly int _column;

        public enum VarTypeEnum
        {
            Input,
            Output,
            Aux
        }

        protected InstructionNode(string var, int line, int column = -1)
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

            _line = line;
            _column = column;
        }

        public int Line => _line;

        public int Column => _column;

        public string Label { get; internal set; }

        public VarTypeEnum VarType { get; }

        public int VarIndex { get; }
    }
}