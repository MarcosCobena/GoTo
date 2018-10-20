using System;
using System.Reflection.Emit;

namespace GoTo.Features.CodeGenerator
{
    static class ILGeneratorExtensions
    {
        public static void EmitArrayValueAtIndex(this ILGenerator il, LocalBuilder array, int index)
        {
            il.Emit(OpCodes.Ldloc, array);
            il.Emit(OpCodes.Ldc_I4, index);
            il.Emit(OpCodes.Ldelem_I4);
        }

        public static void EmitBinaryExpression(
            this ILGenerator il, LocalBuilder array, OpCode operation, int index, Type resultType)
        {
            il.EmitArrayValueAtIndex(array, index);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(operation);
            var tempVar = il.DeclareLocal(resultType);
            il.Emit(OpCodes.Stloc, tempVar);

            il.Emit(OpCodes.Ldloc, array);
            il.Emit(OpCodes.Ldc_I4, index);
            il.Emit(OpCodes.Ldloc, tempVar);
            il.Emit(OpCodes.Stelem_I4);
        }

        public static LocalBuilder EmitNewLocalArray(
            this ILGenerator il, Type arrayType, Type arrayElementType, int length)
        {
            var array = il.DeclareLocal(arrayType);
            il.Emit(OpCodes.Ldc_I4, length);
            il.Emit(OpCodes.Newarr, arrayElementType);
            il.Emit(OpCodes.Stloc, array);

            return array;
        }
    }
}
