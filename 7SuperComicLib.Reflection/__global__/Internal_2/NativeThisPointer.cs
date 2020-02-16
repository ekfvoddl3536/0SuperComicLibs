using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Reflection
{
    internal sealed class NativeThisPointer
    {
        // 의미없는 값
        private static readonly IntPtr magic = new IntPtr(0x1345789A);
        private static bool hasThisPtr;

        #region 준비용
#pragma warning disable
        /// <summary>
        /// 이 메서드는 replacemethod가 잘 작동하는지 확인하기 위한 수단으로 사용됩니다
        /// 다른 메서드를 침범하지 않도록 하기 위해 최소 (32비트: 6바이트, 64비트: 12바이트) 이상의 코드 크기를 유지해야합니다
        /// (어셈블리 기준)
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private Test GetTest(IntPtr t1, IntPtr t2) =>
            throw new NotImplementedException();

        private static void ReplacedTest(NativeThisPointer instance, IntPtr ptr, IntPtr t1, IntPtr t2) =>
            hasThisPtr = t1 == t2 && t1 == magic;
#pragma warning restore
        #endregion

        #region 정적 기본
        internal static bool NeedsPointerFix(Type retType)
        {
            if (retType.IsStruct() == false)
                return false;
            int size = Marshal.SizeOf(retType);
            return
                size < 3 || size == 4 || size == 8
                ? false
                : hasThisPtr;
        }

        // 한 번만 한다
        static NativeThisPointer()
        {
            NativeThisPointer instance = new NativeThisPointer();
            Type me = instance.GetType();

            RuntimeMethodHandle handle = me.GetMethod(nameof(ReplacedTest), Helper.mflag1).MethodHandle;
            RuntimeHelpers.PrepareMethod(handle);
            Helper.ReplaceMethod(me.GetMethod(nameof(GetTest), Helper.mflag0), handle.GetFunctionPointer());

            instance.GetTest(magic, magic);
        }
        #endregion

#pragma warning disable
        private struct Test
        {
            readonly byte b1;
            readonly byte b2;
            readonly byte b3;
        }
#pragma warning restore
    }
}
