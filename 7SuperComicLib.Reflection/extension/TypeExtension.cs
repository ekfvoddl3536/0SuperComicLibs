using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SuperComicLib.Runtime
{
    public static class TypeExtension
    {
        public static bool IsNative(this Type type) => type.IsByRef || type.IsPointer;

        public static bool IsStruct(this Type type) =>
            type.IsValueType && type.IsPrimitive == false && type.IsEnum == false && type != typeof(void);

        #region unmanaged
        public static bool IsUnmanaged_unsafe(this Type type)
        {
            try
            {
                typeof(K<>).MakeGenericType(type);
                return true;
            }
#pragma warning disable
            catch { return false; }
#pragma warning restore
        }

        private static class K<T> where T : unmanaged { }
        #endregion

        #region sp method
        private const string
            opIm = "op_Implicit",
            opEx = "op_Explicit",
            opLn = "op_LogicalNot",
            opOc = "op_OnesComplement"; // 단항

        private const string
            opAdd = "op_Addition",
            opSub = "op_Subtraction",
            opMul = "op_Multiply",
            opDiv = "op_Division",
            opMod = "op_Modulus",
            opOR = "op_BitwiseOr",
            opAND = "op_BitwiseAnd",
            opXOR = "op_ExclusiveOr";

        private const string
            opEq = "op_Equality",
            opIEq = "op_Inequality",
            opGTOE = "op_GreaterThanOrEqual",
            opLTOE = "op_LessThanOrEqual",
            opGt = "op_GreaterThan",
            opLt = "op_LessThan";

        private const string
            opLS = "op_LeftShift",
            opRS = "op_RightShift"; // 인수 0번이 자기자신, 1번은 무조건 int = 여러개 정의 불가능 일반검색

        private const BindingFlags bflag = BindingFlags.Public | BindingFlags.Static;

        #region 단항
        public static MethodInfo Op_Implicit(this Type type, Type returnType) =>
            Op_Implicit(type, returnType, type);

        public static MethodInfo Op_Implicit(this Type type, Type returnType, Type parameterType) =>
            GetSpMethod(type, returnType, parameterType, opIm);

        public static MethodInfo Op_Explicit(this Type type, Type returnType) =>
            Op_Explicit(type, returnType, type);

        public static MethodInfo Op_Explicit(this Type type, Type returnType, Type parameterType) =>
            GetSpMethod(type, returnType, parameterType, opEx);

        public static MethodInfo Op_LogicNot(this Type type) =>
            type.GetMethod(opLn, bflag, null, new Type[1] { type }, null);

        public static MethodInfo Op_Bitwise(this Type type) =>
            type.GetMethod(opOc, bflag, null, new Type[1] { type }, null);
        #endregion

        #region 이항 (기본연산)
        public static MethodInfo Op_Add(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opAdd);

        public static MethodInfo Op_Sub(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opSub);

        public static MethodInfo Op_Mul(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opMul);

        public static MethodInfo Op_Div(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opDiv);

        public static MethodInfo Op_Mod(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opMod);

        public static MethodInfo Op_OR(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opOR);

        public static MethodInfo Op_AND(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opAND);

        public static MethodInfo Op_XOR(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opXOR);
        #endregion

        #region 같음비교
        public static MethodInfo Op_Equal(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opEq);

        public static MethodInfo Op_Inequal(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opIEq);

        public static MethodInfo Op_GreaterThanOrEqual(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opGTOE);

        public static MethodInfo Op_LessThanOrEqual(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opLTOE);

        public static MethodInfo Op_GreaterThan(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opGt);

        public static MethodInfo Op_LessThan(this Type type, Type left, Type right) =>
            FMeth(type, left, right, opLt);
        #endregion

        #region 비트
        public static MethodInfo Op_LShift(this Type type) =>
            FMeth(type, type, typeof(int), opLS);

        public static MethodInfo Op_RShift(this Type type) =>
            FMeth(type, type, typeof(int), opRS);
        #endregion

        private static MethodInfo GetSpMethod(this Type type, Type returnType, Type parameterType, string name)
        {
            MethodInfo[] vs = type.GetMethods(bflag);
            int x = vs.Length;

            while (--x >= 0)
            {
                MethodInfo info = vs[x];
                if (info.Name == name &&
                    info.ReturnType == returnType &&
                    info.GetParameters()[0].ParameterType == parameterType)
                    return info;
            }

            return null;
        }

        private static MethodInfo FMeth(this Type type, Type p1, Type p2, string name) =>
            type.GetMethod(name, bflag, null, new Type[2] { p1, p2 }, null);
        #endregion

        #region size
        public static unsafe int PrimitiveSizeOf(this Type type, out bool isUnsigned, out bool isInteger)
        {
            isUnsigned = IsUnsigned(type); // ulong uint ushort + byte
            isInteger = IsInteger(type); // single(float) OR double

            return Marshal.SizeOf(type);
        }

        public static bool IsUnsigned(this Type type)
        {
            char c = type.Name[0];
            return c == 'U' || c == 'B';
        }

        public static bool IsInteger(this Type type)
        {
            string tmp = type.Name;
            char c = tmp[0];
            return
                c != 'S' && tmp[1] != 'i' && // 'Si'ngle <-> 'S'Byte
                c != 'D'; // 'D'ouble
        }
        #endregion
    }
}
