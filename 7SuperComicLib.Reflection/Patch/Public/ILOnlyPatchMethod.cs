using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Reflection.Patch
{
    public sealed class ILOnlyPatchMethod : ExMethodInfo
    {
        public ILOnlyPatchMethod(MethodInfo info) : base(info) { }

        public override bool GenerateCode(ILGenerator il, MethodBase original, ParameterInfo[] parameters, int offset, bool hasReturn, bool hasArgBuffer)
        {
#if DEBUG
            if (toTypes == null || toTypes.Length < 4)
                // 인수 길이가 충분하지 않습니다
                throw new InvalidOperationException("Parameter length is not long enough");
            if (toTypes[0] != typeof(ILGenerator) || original.GetType().IsAssignableFrom(toTypes[1]) || toTypes[2] != typeof(int) || toTypes[3] != Helper.BOOL_T)
                // ILOnly의 패치함수는 반드시 (ILGenerator, <MethodInfo/ConstructorInfo>, int, bool, bool) 인수를 가져야합니다
                throw new InvalidOperationException("ILOnly patch method must have parameters (ILGenerator, <MethodBase/MethodInfo/ConstructorInfo>, int, bool)");
            if (patching.ReturnType != Helper.VOID_T && patching.ReturnType != Helper.BOOL_T)
                // 패치 메소드의 반환은 void 또는 bool이어야 합니다
                throw new InvalidOperationException("Return type of the patch method must be void or bool");
#endif
            if (patching.ReturnType == Helper.VOID_T)
            {
                patching.Invoke(null, new object[4] { il, original, offset, hasReturn });
                return false;
            }
            else
                return (bool)patching.Invoke(null, new object[4] { il, original, offset, hasReturn });
        }
    }
}
