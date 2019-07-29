using GoTo.Parser.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLabel = System.Reflection.Emit.Label;

namespace GoTo.Emitter
{
    class ILEmitter
    {
        static readonly Type _arrayType = typeof(int[]);
        static readonly Type _arrayElementType = typeof(int);

        static IDictionary<string, ILLabel> _labels = new Dictionary<string, ILLabel>();

        public static void CreateAssembly(ProgramNode program, string outputType, string outputPath)
        {
            ActualCreateAssembly(program, outputType, out AssemblyBuilder assemblyBuilder);
#if !NETSTANDARD
            assemblyBuilder.Save(outputPath);
#endif
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
#if NETSTANDARD
                AssemblyBuilderAccess.Run
#else
                isTransient ? AssemblyBuilderAccess.Run : AssemblyBuilderAccess.Save
#endif
                );
            var moduleBuilder =
#if NETSTANDARD
                assemblyBuilder.DefineDynamicModule(assemblyName.Name);
#else
                isTransient ? 
                    assemblyBuilder.DefineDynamicModule(assemblyName.Name) :
                    assemblyBuilder.DefineDynamicModule(assemblyName.Name, $"{outputType}.dll");
#endif
            var typeBuilder = moduleBuilder.DefineType(
                $"{Framework.OutputNamespace}.{outputType}", TypeAttributes.Public | TypeAttributes.Class);
            var inputType = typeof(int);
            var methodBuilder = typeBuilder.DefineMethod(
                Framework.OutputMethodName,
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(int),
                new Type[] { inputType, inputType, inputType, inputType, inputType, inputType, inputType, inputType });

            for (int parameterIndex = 1; parameterIndex <= Settings.InputAndAuxVarsLength; parameterIndex++)
            {
                var parameterBuilder = methodBuilder.DefineParameter(
                    parameterIndex,
                    ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                    $"x{parameterIndex}");
                parameterBuilder.SetConstant(0);
            }

            var il = methodBuilder.GetILGenerator();

            TranslateInto(program, il);

            var resultingType = typeBuilder.
#if NETSTANDARD
                CreateTypeInfo();
#else
                CreateType();
#endif

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
                    ILLabel label;
                    var rawLabel = item.Label.ToString();

                    if (_labels.ContainsKey(rawLabel))
                    {
                        label = _labels[rawLabel];
                    }
                    else
                    {
                        label = il.DefineLabel();
                        _labels.Add(rawLabel, label);
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

            if (_labels.TryGetValue(Settings.ExitLabelId.ToString(), out ILLabel exitLabel))
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
                _labels.Add(item.TargetLabel.ToString(), label);
            }

            if (!_labels.ContainsKey(Settings.ExitLabelId.ToString()))
            {
                var exitLabel = il.DefineLabel();
                _labels.Add(Settings.ExitLabelId.ToString(), exitLabel);
            }
        }

        static void TranslateInstruction(
            ILGenerator il, 
            (LocalBuilder x, LocalBuilder y, LocalBuilder z) vars, 
            ConditionalInstructionNode node)
        {
            PushVar(il, node, vars);
            il.Emit(OpCodes.Ldc_I4_0);

            ILLabel label;
            var rawTargetLabel = node.TargetLabel.ToString();

            if (_labels.ContainsKey(rawTargetLabel))
            {
                label = _labels[rawTargetLabel];
            }
            else
            {
                label = il.DefineLabel();
                _labels.Add(rawTargetLabel, label);
            }

            il.Emit(OpCodes.Bne_Un, label);
        }

        static void PushVar(
            ILGenerator il, InstructionNode node, (LocalBuilder x, LocalBuilder y, LocalBuilder z) vars)
        {
            var var = node.Var;
            var relativeVarIndex = var.Index - 1;

            switch (var.Type)
            {
                case Var.VarTypeEnum.Input:
                    il.EmitArrayValueAtIndex(vars.x, relativeVarIndex);
                    break;
                case Var.VarTypeEnum.Output:
                    il.Emit(OpCodes.Ldloc, vars.y);
                    break;
                case Var.VarTypeEnum.Aux:
                    il.EmitArrayValueAtIndex(vars.z, relativeVarIndex);
                    break;
                default:
                    throw new ArgumentException(
                        $"Unrecognized var type: {var.Type}", nameof(node));
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
            var var = node.Var;
            var relativeVarIndex = var.Index - 1;

            switch (var.Type)
            {
                case Var.VarTypeEnum.Input:
                    il.EmitBinaryExpression(vars.x, operation, relativeVarIndex, _arrayElementType);
                    break;
                case Var.VarTypeEnum.Output:
                    TranslateSubBinaryExpressionOnOutputVar(il, vars.y, operation);
                    break;
                case Var.VarTypeEnum.Aux:
                    il.EmitBinaryExpression(vars.z, operation, relativeVarIndex, _arrayElementType);
                    break;
                default:
                    throw new ArgumentException(
                        $"Unrecognized var type: {var.Type}", nameof(node));
            }
        }

        static (LocalBuilder x, LocalBuilder y, LocalBuilder z) EmitLocalVars(ILGenerator il)
        {
            var xInputVars = il.EmitNewLocalArray(_arrayType, _arrayElementType, Settings.InputAndAuxVarsLength);

            for (var i = 0; i < Settings.InputAndAuxVarsLength; i++)
            {
                il.Emit(OpCodes.Ldloc, xInputVars);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i);
                il.Emit(OpCodes.Stelem_I4);
            }

            var yOutputVar = il.DeclareLocal(_arrayElementType);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, yOutputVar);

            var zAuxVars = il.EmitNewLocalArray(_arrayType, _arrayElementType, Settings.InputAndAuxVarsLength);

            return (xInputVars, yOutputVar, zAuxVars);
        }

        static void TranslateSubBinaryExpressionOnOutputVar(ILGenerator il, LocalBuilder y, OpCode operation)
        {
            var alreadyOneLabel = il.DefineLabel();

            if (operation == OpCodes.Sub)
            {
                il.Emit(OpCodes.Ldloc, y);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Blt, alreadyOneLabel);
            }

            il.Emit(OpCodes.Ldloc, y);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(operation);
            il.Emit(OpCodes.Stloc, y);

            if (operation == OpCodes.Sub)
            {
                il.MarkLabel(alreadyOneLabel);
            }
        }
    }
}
