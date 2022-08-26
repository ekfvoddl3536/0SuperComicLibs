#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
using System;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref partial struct NativeConstSpan<T> where T : unmanaged
    {
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString()
        {
            if (typeof(T) == typeof(char))
#if AnyCPU || X86
                return new string((char*)Source, 0, Length);
#else
                return new string((char*)Source, 0, (int)Length);
#endif

            return $"{nameof(SuperComicLib)}.{nameof(NativeConstSpan<T>)}<{nameof(T)}>[{Length}]";
        }
    }
}
