using System.Runtime.CompilerServices;

namespace SuperComicLib.Core
{
    public sealed class Share<T>
        where T : struct
    {
        public T Value;

        public Share(T initValue) => Value = initValue;

        public Share(in T ref_initValue) => Value = ref_initValue;

        public override bool Equals(object obj) => Value.Equals(obj);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();

        public static T? GetNullableValue(Share<T> inst) => !inst;
        public static T GetValueSafe(Share<T> inst) => +inst;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Share<T>(T v) => new Share<T>(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? operator !(Share<T> inst) => inst?.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator +(Share<T> inst) => 
            inst == null 
            ? default 
            : inst.Value;
    }
}
