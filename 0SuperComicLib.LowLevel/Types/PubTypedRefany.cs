using System;
using TR = System.TypedReference;

namespace SuperComicLib.LowLevel
{
    public readonly unsafe struct PubTypedRefany
    {
        public readonly IntPtr Value;
        public readonly IntPtr Type;

        /// <summary>
        /// ptr = TypedReference*
        /// </summary>
        public PubTypedRefany(IntPtr* ptr)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));

            Value = ptr[0];
            Type = ptr[1];
        }

        public bool IsInvalid => Value == IntPtr.Zero || Type == IntPtr.Zero;

        public override bool Equals(object obj) =>
            obj is PubTypedRefany other ?
            this == other
            : false;

        public override unsafe int GetHashCode() => 
            Type == IntPtr.Zero
            ? 0
            : Refanytype(this).GetHashCode();

        public static bool operator ==(PubTypedRefany left, PubTypedRefany right) => 
            left.Value == right.Value &&
            left.Type == right.Type;

        public static bool operator !=(PubTypedRefany left, PubTypedRefany right) =>
            left.Value != right.Value ||
            left.Type != right.Type;

        public static PubTypedRefany Create<T>(T value)
        {
            if (value == null)
                return default;
            else
            {
                TR tr = __makeref(value);
                return new PubTypedRefany((IntPtr*)&tr);
            }
        }

        public static T Refanyval<T>(PubTypedRefany value) => __refvalue(*(TR*)&value, T);

        public static Type Refanytype(PubTypedRefany value) => __reftype(*(TR*)&value);

        public static bool TryRefanyval<T>(PubTypedRefany value, out T result)
        {
            if (value.IsInvalid)
            {
                result = default;
                return false;
            }
            else
            {
                result = __refvalue(*(TR*)&value, T);
                return true;
            }
        }

        public static bool TryRefanytype(PubTypedRefany value, out Type result)
        {
            if (value.Type == IntPtr.Zero)
            {
                result = null;
                return false;
            }
            else
            {
                result = __reftype(*(TR*)&value);
                return true;
            }
        }
    }
}
