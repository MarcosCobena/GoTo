using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GoTo.Emitter
{
    class ILEmitter
    {
        const string ExitLabel = "E";
        const int InputAndAuxVarsLength = 8;

        static readonly Type _arrayType = typeof(int[]);
        static readonly Type _arrayElementType = typeof(int);

        static IDictionary<string, Label> _labels = new Dictionary<string, Label>();

        public static void CreateAssembly(ProgramNode program, string outputType, string outputPath)
        {
            ActualCreateAssembly(program, outputType, out AssemblyBuilder assemblyBuilder);
            assemblyBuilder.Save(outputPath);
        }

        public static Type CreateType(ProgramNode program, string outputType) => 
            ActualCreateAssembly(program, outputType, out AssemblyBuilder assemblyBuilder, isTransient: true);

        static Type ActualCreateAssembly(
            ProgramNode program, string outputType, out AssemblyBuilder assemblyBuilder, bool isTransient = false)
        {
            var assemblyName = new AssemblyName(outputType);
            var appDomain = AppDomain.CurrentDomain;
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                isTransient ? AssemblyBuilderAccess.Run : AssemblyBuilderAccess.Save);
            var moduleBuilder = isTransient ? 
                assemblyBuilder.DefineDynamicModule(assemblyName.Name) :
                assemblyBuilder.DefineDynamicModule(assemblyName.Name, $"{outputType}.dll");
            var typeBuilder = moduleBuilder.DefineType(
                $"{Language.OutputNamespace}.{outputType}", TypeAttributes.Public | TypeAttributes.Class);
            var inputType = typeof(int);
            var methodBuilder = typeBuilder.DefineMethod(
                Language.OutputMethodName,
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(int),
                new Type[] { inputType, inputType, inputType, inputType, inputType, inputType, inputType, inputType });

            for (int parameterIndex = 1; parameterIndex <= InputAndAuxVarsLength; parameterIndex++)
            {
                var parameterBuilder = methodBuilder.DefineParameter(
                    parameterIndex,
                    ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                    $"x{parameterIndex}");
                parameterBuilder.SetConstant(0);
            }

            var il = methodBuilder.GetILGenerator();

            TranslateInto(program, il);

            var resultingType = typeBuilder.CreateType();

            return resultingType;
        }

        static void TranslateInto(ProgramNode program, ILGenerator il)
        {
            CreateTargetLabels(il, program);

            var vars = EmitLocalVars(il);

            foreach (var item in program.Instructions)
            {
                if (item.Label != null)
                {
                    Label label;

                    if (_labels.ContainsKey(item.Label))
                    {
                        label = _labels[item.Label];
                    }
                    else
                    {
                        label = il.DefineLabel();
                        _labels.Add(item.Label, label);
                    }

                    il.MarkLabel(label);
                }

                switch (item)
                {
                    case BinaryExpressionInstructionNode node:
                        TranslateInstruction(il, vars, node);
                        break;
                    case UnaryExpressionInstructionNode _:
                        il.Emit(OpCodes.Nop);
                        break;
                    case ConditionalInstructionNode node:
                        TranslateInstruction(il, vars, node);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (_labels.TryGetValue(ExitLabel, out Label exitLabel))
            {
                il.MarkLabel(exitLabel);
            }

            il.Emit(OpCodes.Ldloc, vars.y);
            il.Emit(OpCodes.Ret);
        }

        static void CreateTargetLabels(ILGenerator il, ProgramNode program)
        {
            _labels.Clear();

            var conditionalInstructions = program.Instructions
                .Where(instruction => instruction is ConditionalInstructionNode)
                .Cast<ConditionalInstructionNode>();

            foreach (var item in conditionalInstructions)
            {
                var label = il.DefineLabel();
                _labels.Add(item.TargetLabel, label);
            }

            if (!_labels.ContainsKey(ExitLabel))
            {
                var exitLabel = il.DefineLabel();
                _labels.Add(ExitLabel, exitLabel);
            }
        }

        static void TranslateInstruction(
            ILGenerator il, 
            (LocalBuilder x, LocalBuilder y, LocalBuilder z) vars, 
            ConditionalInstructionNode node)
        {
            PushVar(il, node, vars);
            il.Emit(OpCodes.Ldc_I4_0);

            Label label;

            if (_labels.ContainsKey(node.TargetLabel))
            {
                label = _labels[node.TargetLabel];
            }
            else
            {
                label = il.DefineLabel();
                _labels.Add(node.TargetLabel, label);
            }

            il.Emit(OpCodes.Bne_Un, label);
        }

        static void PushVar(
            ILGenerator il, InstructionNode node, (LocalBuilder x, LocalBuilder y, LocalBuilder z) vars)
        {
            var relativeVarIndex = node.VarIndex - 1;

            switch (node.VarType)
            {
                case InstructionNode.VarTypeEnum.Input:
                    il.EmitArrayValueAtIndex(vars.x, relativeVarIndex);
                    break;
                case InstructionNode.VarTypeEnum.Output:
                    il.Emit(OpCodes.Ldloc, vars.y);
                    break;
                case InstructionNode.VarTypeEnum.Aux:
                    il.EmitArrayValueAtIndex(vars.z, relativeVarIndex);
                    break;
                default:
                    throw new ArgumentException($"Unrecognized var type: {node.VarType}", nameof(node));
            }
        }

        static void TranslateInstruction(
            ILGenerator il, 
            (LocalBuilder x, LocalBuilder y, LocalBuilder z) vars, 
            BinaryExpressionInstructionNode node)
        {
            var operation = node.Operator == BinaryExpressionInstructionNode.OperatorEnum.Increment ?
                OpCodes.Add :
                OpCodes.Sub;
            var relativeVarIndex = node.VarIndex - 1;

            switch (node.VarType)
            {
                case InstructionNode.VarTypeEnum.Input:
                    il.EmitBinaryExpression(vars.x, operation, relativeVarIndex, _arrayElementType);
                    break;
                case InstructionNode.VarTypeEnum.Output:
                    il.Emit(OpCodes.Ldloc, vars.y);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(operation);
                    il.Emit(OpCodes.Stloc, vars.y);
                    break;
                case InstructionNode.VarTypeEnum.Aux:
                    il.EmitBinaryExpression(vars.z, operation, relativeVarIndex, _arrayElementType);
                    break;
                default:
                    throw new ArgumentException($"Unrecognized var type: {node.VarType}", nameof(node));
            }
        }

        static (LocalBuilder x, LocalBuilder y, LocalBuilder z) EmitLocalVars(ILGenerator il)
        {
            var xInputVars = il.EmitNewLocalArray(_arrayType, _arrayElementType, InputAndAuxVarsLength);

            for (var i = 0; i < InputAndAuxVarsLength; i++)
            {
                il.Emit(OpCodes.Ldloc, xInputVars);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i);
                il.Emit(OpCodes.Stelem_I4);
            }

            var yOutputVar = il.DeclareLocal(_arrayElementType);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, yOutputVar);

            var zAuxVars = il.EmitNewLocalArray(_arrayType, _arrayElementType, InputAndAuxVarsLength);

            return (xInputVars, yOutputVar, zAuxVars);
        }
    }
}
