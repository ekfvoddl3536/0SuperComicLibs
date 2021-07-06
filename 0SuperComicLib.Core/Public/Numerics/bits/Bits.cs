using System;

namespace SuperComicLib.Numerics
{
    public abstract class Bits : IDisposable, IEquatable<Bits>, ICloneable
    {
        protected Bits() { }

        #region method 1
        public abstract int Length { get; }

        public abstract bool this[int bitPosition] { get; set; }

        public abstract void MarkBit(int bitPosition);

        public abstract void Clear();

        public abstract void SetAll(bool value);
        #endregion

        #region method 2
        public abstract void AND(Bits other);

        public abstract void OR(Bits other);

        public abstract void XOR(Bits other);

        public abstract void NOT();

        public abstract void LSHIFT(int count);

        public abstract void RSHIFT(int count);
        #endregion

        #region method 3
        public abstract bool Equals(Bits other);

        public abstract void CopyFrom(Bits other);

        public abstract Bits Clone();

        object ICloneable.Clone() => Clone();

        public abstract uint[] ToArray();
        #endregion

        #region disposable
        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region operator
        public static Bits operator &(Bits left, Bits right)
        {
            Bits v = left.Clone();
            v.AND(right);
            return v;
        }
        public static Bits operator |(Bits left, Bits right)
        {
            Bits v = left.Clone();
            v.OR(right);
            return v;
        }
        public static Bits operator ^(Bits left, Bits right)
        {
            Bits v = left.Clone();
            v.XOR(right);
            return v;
        }
        public static Bits operator ~(Bits left)
        {
            Bits v = left.Clone();
            v.NOT();
            return v;
        }
        public static Bits operator >>(Bits left, int shift)
        {
            Bits v = left.Clone();
            v.RSHIFT(shift);
            return v;
        }
        public static Bits operator <<(Bits left, int shift)
        {
            Bits v = left.Clone();
            v.LSHIFT(shift);
            return v;
        }
        #endregion
    }
}
