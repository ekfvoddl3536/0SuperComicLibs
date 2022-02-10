using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Core
{
    [System.Obsolete("do not use bool2int class", true)]
    public static class Bool2Int
    {
        internal delegate int work(bool x);

        internal static readonly work _func;

#pragma warning disable
        static Bool2Int()
        {
            DynamicMethod dm = new DynamicMethod(
                "__SCLC__bool2int_il", 
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                CTypes.INT_T,
                new[] { CTypes.BOOL_T },
                typeof(Bool2Int).Module,
                true);

            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);

            _func = (work)dm.CreateDelegate(typeof(work));
        }
#pragma warning restore

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Convert(bool value) => _func(value);
    }
}
