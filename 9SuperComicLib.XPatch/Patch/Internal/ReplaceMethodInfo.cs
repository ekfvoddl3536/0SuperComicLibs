using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    internal sealed class ReplaceMethodInfo : ExMethodInfo
    {
        public ReplaceMethodInfo(MethodInfo info) : base(info) { }

        public override bool GenerateCode(
            ILGenerator il,
            MethodBase original,
            ParameterInfo[] parameters,
            int offset,
            bool hasReturn,
            bool hasArgBuffer,
            bool isValuetype)
        {
            if (original.IsStatic == false)
                il.Emit(OpCodes.Ldarg_0);

            foreach (ParameterInfo p in parameters)
                if (p.ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldarga, (short)(p.Position + offset));
                else
                    il.Emit(OpCodes.Ldarg, (short)(p.Position + offset));

            il.Emit(OpCodes.Call, patching);

            return false;
        }
    }
}