using GoTo.Features.AbstractSyntaxTree;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GoTo.Features.CodeGenerator
{
    class CodeGenerator
    {
        const string Namespace = "GoTo";
        const string MethodName = "Run";

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
            var ilGenerator = methodBuilder.GetILGenerator();

            TranslateInto(program, ilGenerator);

            Type resultingType = typeBuilder.CreateTypeInfo();

            return resultingType;
        }

        static void TranslateInto(ProgramNode program, ILGenerator ilGenerator)
        {
            var yOutputVar = ilGenerator.DeclareLocal(typeof(int));
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Stloc, yOutputVar);

            foreach (var item in program.Instructions)
            {
                switch (item)
                {
                    case BinaryExpressionInstructionNode node:
                        if (node.Label != null)
                        {
                            var label = ilGenerator.DefineLabel();
                            ilGenerator.MarkLabel(label);
                        }

                        ilGenerator.Emit(OpCodes.Ldloc, yOutputVar);
                        ilGenerator.Emit(OpCodes.Ldc_I4_1);

                        // TODO compare with enum
                        var operation = node.Operator == "+" ? 
                            OpCodes.Add : 
                            OpCodes.Sub;

                        ilGenerator.Emit(operation);
                        ilGenerator.Emit(OpCodes.Stloc, yOutputVar);
                        break;
                    case UnaryExpressionInstructionNode _:
                        ilGenerator.Emit(OpCodes.Nop);
                        break;
                    case ConditionalInstructionNode _:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            ilGenerator.Emit(OpCodes.Ldloc, yOutputVar);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
