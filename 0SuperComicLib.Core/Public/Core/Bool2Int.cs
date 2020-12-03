using System.Reflection.Emit;

namespace SuperComicLib.Core
{
    public static class Bool2Int
    {
        internal delegate int work(bool x);

        internal static readonly work _func;

#pragma warning disable
        static Bool2Int()
        {
            DynamicMethod dm = new DynamicMethod("__SCLC__bool2int_il", CTypes.INT_T, new[] { CTypes.BOOL_T });

            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);

            _func = (work)dm.CreateDelegate(typeof(work));
        }
#pragma warning restore

        public static int Convert(bool value) => _func(value);
    }
}
