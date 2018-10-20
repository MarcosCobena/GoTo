using GoTo.Features.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GoTo.Features.CodeGenerator
{
    class CodeGenerator
    {
        const int InputAndAuxVarsLength = 8;
        const string Namespace = "GoTo";
        const string MethodName = "Run";

        static IDictionary<string, Label> _labels = new Dictionary<string, Label>();

        public static Assembly CreateAssembly(ProgramNode program, string outputType)
        {
            ActualCreateAssembly(program, outputType, out AssemblyBuilder assemblyBuilder);

#if !NETSTANDARD
            //assemblyBuilder.Save("Hello.exe");
#endif

            throw new NotImplementedException();
        }

        public static Type CreateType(ProgramNode program, string outputType) => 
            ActualCreateAssembly(program, outputType, out AssemblyBuilder assemblyBuilder);

        static Type ActualCreateAssembly(ProgramNode program, string outputType, out AssemblyBuilder assemblyBuilder)
        {
            var assemblyName = new AssemblyName(outputType);
            var appDomain = AppDomain.CurrentDomain;
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName,
#if NETSTANDARD
                AssemblyBuilderAccess.Run
#endif
                );
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name
#if !NETSTANDARD
                , "TODO.exe"
#endif
                );
            var typeBuilder = moduleBuilder.DefineType(
                $"{Namespace}.{outputType}", TypeAttributes.Public | TypeAttributes.Class);
            var inputType = typeof(int);
            var methodBuilder = typeBuilder.DefineMethod(
                MethodName,
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

            Type resultingType = typeBuilder.CreateTypeInfo();

            return resultingType;
        }

        static void TranslateInto(ProgramNode program, ILGenerator il)
        {
            _labels.Clear();

            var vars = InitializeVars(il);

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

            il.Emit(OpCodes.Ldloc, vars.y);
            il.Emit(OpCodes.Ret);
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
                    {
                        // x[index - 1]
                        il.Emit(OpCodes.Ldloc, vars.x);
                        il.Emit(OpCodes.Ldc_I4, relativeVarIndex);
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    }
                case InstructionNode.VarTypeEnum.Output:
                    {
                        il.Emit(OpCodes.Ldloc, vars.y);
                        break;
                    }
                case InstructionNode.VarTypeEnum.Aux:
                    {
                        // z[index - 1]
                        il.Emit(OpCodes.Ldloc, vars.z);
                        il.Emit(OpCodes.Ldc_I4, relativeVarIndex);
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    }
                default:
                    throw new ArgumentException($"Unrecognized var type: {node.VarType}", nameof(node));
            }
        }

        static void TranslateInstruction(
            ILGenerator il, 
            (LocalBuilder x, LocalBuilder y, LocalBuilder z) vars, 
            BinaryExpressionInstructionNode node)
        {
            var relativeVarIndex = node.VarIndex - 1;

            switch (node.VarType)
            {
                case InstructionNode.VarTypeEnum.Input:
                    {
                        PushVar(il, node, vars);
                        il.Emit(OpCodes.Ldc_I4_1);

                        var operation = node.Operator == BinaryExpressionInstructionNode.OperatorEnum.Increment ?
                            OpCodes.Add :
                            OpCodes.Sub;

                        il.Emit(operation);
                        var tempVar = il.DeclareLocal(typeof(int));
                        il.Emit(OpCodes.Stloc, tempVar);

                        il.Emit(OpCodes.Ldloc, vars.x);
                        il.Emit(OpCodes.Ldc_I4, relativeVarIndex);
                        il.Emit(OpCodes.Ldloc, tempVar);
                        il.Emit(OpCodes.Stelem_I4);

                        break;
                    }
                case InstructionNode.VarTypeEnum.Output:
                    {
                        PushVar(il, node, vars);
                        il.Emit(OpCodes.Ldc_I4_1);

                        var operation = node.Operator == BinaryExpressionInstructionNode.OperatorEnum.Increment ?
                            OpCodes.Add :
                            OpCodes.Sub;

                        il.Emit(operation);
                        il.Emit(OpCodes.Stloc, vars.y);

                        break;
                    }
                case InstructionNode.VarTypeEnum.Aux:
                    {
                        // FIXME make the same as with x above
                        il.Emit(OpCodes.Ldloc, vars.z);
                        il.Emit(OpCodes.Ldc_I4, relativeVarIndex);

                        PushVar(il, node, vars);
                        il.Emit(OpCodes.Ldc_I4_1);

                        var operation = node.Operator == BinaryExpressionInstructionNode.OperatorEnum.Increment ?
                            OpCodes.Add :
                            OpCodes.Sub;

                        il.Emit(operation);

                        il.Emit(OpCodes.Stelem_I4);

                        break;
                    }
                default:
                    throw new ArgumentException($"Unrecognized var type: {node.VarType}", nameof(node));
            }
        }

        static (LocalBuilder x, LocalBuilder y, LocalBuilder z) InitializeVars(ILGenerator il)
        {
            var arrayType = typeof(int[]);
            var arrayElementType = typeof(int);

            var xInputVars = il.DeclareLocal(arrayType);
            il.Emit(OpCodes.Ldc_I4, InputAndAuxVarsLength);
            il.Emit(OpCodes.Newarr, arrayElementType);
            il.Emit(OpCodes.Stloc, xInputVars);

            for (var i = 0; i < InputAndAuxVarsLength; i++)
            {
                il.Emit(OpCodes.Ldloc, xInputVars);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i);
                il.Emit(OpCodes.Stelem_I4);
            }

            var yOutputVar = il.DeclareLocal(arrayElementType);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, yOutputVar);

            var zAuxVars = il.DeclareLocal(arrayType);
            il.Emit(OpCodes.Ldc_I4, InputAndAuxVarsLength);
            il.Emit(OpCodes.Newarr, arrayElementType);
            il.Emit(OpCodes.Stloc, zAuxVars);

            return (xInputVars, yOutputVar, zAuxVars);
        }
    }
}
