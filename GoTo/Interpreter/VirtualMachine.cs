﻿using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoTo.Interpreter
{
    internal static class VirtualMachine
    {
        private const int MaxSteps = 2 << 12;

        private static int[] x;
        private static int[] y;
        private static int[] z;

        internal static int Run(
            ProgramNode program, 
            int x1, 
            int x2, 
            int x3, 
            int x4, 
            int x5, 
            int x6, 
            int x7, 
            int x8, 
            Func<Locals, bool> stepDebugAndContinue = null)
        {
            x = new int[Settings.InputAndAuxVarsLength];
            x[0] = x1;
            x[1] = x2;
            x[2] = x3;
            x[3] = x4;
            x[4] = x5;
            x[5] = x6;
            x[6] = x7;
            x[7] = x8;
            y = new int[1];
            z = new int[Settings.InputAndAuxVarsLength];

            var instructionPointer = 0;
            var stepsTaken = 0;

            while (instructionPointer >= 0 && instructionPointer < program.Instructions.Count)
            {
                if (stepsTaken >= MaxSteps)
                {
                    throw new MaxStepsExceededException();
                }

                instructionPointer = Step(instructionPointer, program.Instructions);
                stepsTaken++;

                var currentLocals = new Locals { X = x, Y = y[0], Z = z };

                if (stepDebugAndContinue != null && !stepDebugAndContinue.Invoke(currentLocals))
                {
                    break;
                }
            }

            return y[0];
        }

        private static int Step(int instructionPointer, IEnumerable<InstructionNode> instructions)
        {
            var instruction = instructions.ElementAt(instructionPointer);
            int newInstructionPointer;

            if (instruction is BinaryExpressionInstructionNode binaryExpression)
            {
                StepBinaryExpression(binaryExpression);
                newInstructionPointer = instructionPointer + 1;
            }
            else if (instruction is ConditionalInstructionNode conditional)
            {
                newInstructionPointer = StepConditional(conditional, instructionPointer, instructions);
            }
            else // UnaryExpressionInstructionNode
            {
                newInstructionPointer = instructionPointer + 1;
            }

            return newInstructionPointer;
        }

        private static void StepBinaryExpression(BinaryExpressionInstructionNode binaryExpression)
        {
            var targetVar = GetTargetVar(binaryExpression.Var.Type);
            var index = GetNormalizedIndex(binaryExpression);

            switch (binaryExpression.Operator)
            {
                case BinaryExpressionInstructionNode.OperatorEnum.Increment:
                    targetVar[index] += 1;
                    break;
                case BinaryExpressionInstructionNode.OperatorEnum.Decrement:
                    if (targetVar[index] > 0)
                    {
                        targetVar[index] -= 1;
                    }

                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static int StepConditional(
            ConditionalInstructionNode conditional,
            int instructionPointer, 
            IEnumerable<InstructionNode> instructions)
        {
            int newInstructionPointer;
            var targetVar = GetTargetVar(conditional.Var.Type);
            int index = GetNormalizedIndex(conditional);

            if (targetVar[index] != 0)
            {
                var goToInstruction = instructions.FirstOrDefault(item => item.Label == conditional.TargetLabel);

                if (goToInstruction != null)
                {
                    // TODO avoid array conversion
                    newInstructionPointer = Array.IndexOf(instructions.ToArray(), goToInstruction);
                }
                else
                {
                    // We suppose it's the exit label
                    newInstructionPointer = instructions.Count();
                }
            }
            else
            {
                newInstructionPointer = instructionPointer + 1;
            }

            return newInstructionPointer;
        }

        private static int GetNormalizedIndex(InstructionNode instruction) => 
            instruction.Var.Index - 1;

        private static int[] GetTargetVar(Var.VarTypeEnum varType)
        {
            int[] targetVar;

            switch (varType)
            {
                case Var.VarTypeEnum.Input:
                    targetVar = x;
                    break;
                case Var.VarTypeEnum.Output:
                    targetVar = y;
                    break;
                case Var.VarTypeEnum.Aux:
                    targetVar = z;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return targetVar;
        }
    }
}
