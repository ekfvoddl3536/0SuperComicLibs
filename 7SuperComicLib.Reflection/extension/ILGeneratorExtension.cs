using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Reflection
{
    public static class ILGeneratorExtension
    {
        public static void Emit_Ldc_I4(this ILGenerator il, int value)
        {
            if (value == -1)
                il.Emit(OpCodes.Ldc_I4_M1);
            else if (value == 0)
                il.Emit(OpCodes.Ldc_I4_0);
            else if (value == 1)
                il.Emit(OpCodes.Ldc_I4_1);
            else if (value == 2)
                il.Emit(OpCodes.Ldc_I4_2);
            else if (value == 3)
                il.Emit(OpCodes.Ldc_I4_3);
            else if (value == 4)
                il.Emit(OpCodes.Ldc_I4_4);
            else if (value == 5)
                il.Emit(OpCodes.Ldc_I4_5);
            else if (value == 6)
                il.Emit(OpCodes.Ldc_I4_6);
            else if (value == 7)
                il.Emit(OpCodes.Ldc_I4_7);
            else if (value == 8)
                il.Emit(OpCodes.Ldc_I4_8);
            else if (value < sbyte.MaxValue)
                il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
            else
                il.Emit(OpCodes.Ldc_I4, value);
        }

        public static void Emit_Ldc_I8(this ILGenerator il, long value) => il.Emit(OpCodes.Ldc_I8, value);

        public static void Emit_Ldc_R4(this ILGenerator il, float value) => il.Emit(OpCodes.Ldc_R4, value);

        public static void Emit_Ldc_R8(this ILGenerator il, double value) => il.Emit(OpCodes.Ldc_R8, value);

        public static void Emit_Ldstr(this ILGenerator il, string value) => il.Emit(OpCodes.Ldstr, value);

        public static void Emit_Ldarg(this ILGenerator il, int value)
        {
            if (value == 0)
                il.Emit(OpCodes.Ldarg_0);
            else if (value == 1)
                il.Emit(OpCodes.Ldarg_1);
            else if (value == 2)
                il.Emit(OpCodes.Ldarg_2);
            else if (value == 3)
                il.Emit(OpCodes.Ldarg_3);
            else if (value <= byte.MaxValue)
                il.Emit(OpCodes.Ldarg_S, (byte)value);
            else
                il.Emit(OpCodes.Ldarg, (short)value);
        }

        public static void Emit_Ldarga(this ILGenerator il, int value)
        {
            if (value <= byte.MaxValue)
                il.Emit(OpCodes.Ldarga_S, (byte)value);
            else
                il.Emit(OpCodes.Ldarga, (short)value);
        }

        public static void Emit_Ldloc(this ILGenerator il, int value)
        {
            if (value == 0)
                il.Emit(OpCodes.Ldloc_0);
            else if (value == 1)
                il.Emit(OpCodes.Ldloc_1);
            else if (value == 2)
                il.Emit(OpCodes.Ldloc_2);
            else if (value == 3)
                il.Emit(OpCodes.Ldloc_3);
            else if (value <= byte.MaxValue)
                il.Emit(OpCodes.Ldloc_S, (byte)value);
            else
                il.Emit(OpCodes.Ldloc, (short)value);
        }

        public static void Emit_Ldloca(this ILGenerator il, int value)
        {
            if (value <= byte.MaxValue)
                il.Emit(OpCodes.Ldloca_S, (byte)value);
            else
                il.Emit(OpCodes.Ldloca, (short)value);
        }

        public static void Emit_Starg(this ILGenerator il, int value)
        {
            if (value <= byte.MaxValue)
                il.Emit(OpCodes.Starg_S, (byte)value);
            else
                il.Emit(OpCodes.Starg, (short)value);
        }

        public static void Emit_Stloc(this ILGenerator il, int value)
        {
            if (value == 0)
                il.Emit(OpCodes.Stloc_0);
            else if (value == 1)
                il.Emit(OpCodes.Stloc_1);
            else if (value == 2)
                il.Emit(OpCodes.Stloc_2);
            else if (value == 3)
                il.Emit(OpCodes.Stloc_3);
            else if (value <= byte.MaxValue)
                il.Emit(OpCodes.Stloc_S, (byte)value);
            else
                il.Emit(OpCodes.Stloc, (short)value);
        }

        public static void Emit_Ldfld(this ILGenerator il, FieldInfo info)
        {
            if ((info.Attributes & FieldAttributes.Static) != 0)
                il.Emit(OpCodes.Ldsfld, info);
            else
                il.Emit(OpCodes.Ldfld, info);
        }

        public static void Emit_Ldflda(this ILGenerator il, FieldInfo info)
        {
            if ((info.Attributes & FieldAttributes.Static) != 0)
                il.Emit(OpCodes.Ldsflda, info);
            else
                il.Emit(OpCodes.Ldflda, info);
        }

        public static void Emit_Stfld(this ILGenerator il, FieldInfo info)
        {
            if ((info.Attributes & FieldAttributes.Static) != 0)
                il.Emit(OpCodes.Stsfld, info);
            else
                il.Emit(OpCodes.Stfld, info);
        }

        public static void Emit_Ldloc_OR_Ldarg(this ILGenerator il, int index, bool isLocal)
        {
            if (isLocal)
                Emit_Ldloc(il, index);
            else
                Emit_Ldarg(il, index);
        }

        public static void Emit_CallMethod(this ILGenerator il, MethodInfo methinfo)
        {
            if (methinfo.IsVirtual)
                il.Emit(OpCodes.Callvirt, methinfo);
            else
                il.Emit(OpCodes.Call, methinfo);
        }
    }
}
