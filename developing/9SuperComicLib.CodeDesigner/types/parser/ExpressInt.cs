using System;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    // A B C
    public readonly struct ExpressInt : IEquatable<ExpressInt>, IEnumerableSlim<int>
    {
        #region constants
        public const int epsilon = -1;
        public const int nonterminal_flag = 0x4000_0000;
        public const int start_symbol = 0x3F00_0000;
        // public const int end_symbol = start_symbol | int.MinValue; // 0xBF00_0000
        #endregion

        private readonly int[] vars;

        #region constructor
        public ExpressInt(int[] items) => vars = items;

        public ExpressInt(TokenType[] items)
        {
            int x = items.Length;
            int[] vs = new int[x];

            while (--x >= 0)
                vs[x] = (int)items[x];

            vars = vs;
        }

        public ExpressInt(string str, IExpressMap em)
        {
            string[] datas = str.Trim().Split(' ');
            int x = datas.Length;

            int[] vs = new int[x];
            while (--x >= 0)
            {
                str = datas[x].Trim();
                vs[x] =
                    em.Terminal(str, out TokenType result)
                    ? (int)result    // awalys positive
                    : em.Nonterminal(str); // always negative
            }

            vars = vs;
        }
        #endregion

        #region method && property
        public int Length => vars.Length;

        public int this[int index] => vars[index];

        public bool IsInvalid => vars == null;

        public bool IsEpsilon => vars.Length == 1 && vars[0] == epsilon;

        public bool Equals(ExpressInt other)
        {
            int[] me = vars;
            int[] ot = other.vars;

            int x = me.Length;
            if (x == ot.Length)
            {
                while (--x >= 0)
                    if (me[x] != ot[x])
                        return false;
            }

            return true;
        }

        public bool Equals(string str) => Equals(new ExpressInt(str, EnumTM.Default));

        public bool Contains(int other)
        {
            int[] me = vars;
            
            int x = me.Length;
            while (--x >= 0)
                if (me[x] == other)
                    return true;

            return false;
        }

        public bool Contains(TokenType other) => Contains((int)other);

        public bool Contains(ExpressInt other)
        {
            int[] me = vars;
            IForwardIterator<int> ots = other.vars.Begin();

            int x = me.Length;
            while (--x >= 0)
            {
                int cur = me[x];
                do
                {
                    if (ots.Value == cur)
                        return true;
                } while (ots.LazyAdd());
                ots.Reset();
            }

            return false;
        }

        public bool Contains(string str) => Contains(new ExpressInt(str, EnumTM.Default));

        public int[] ToArray()
        {
            int[] src = vars;
            int size = src.Length;

            int[] dst = new int[size];
            Array.Copy(src, dst, size);

            return dst;
        }

        public IForwardIterator<int> Begin() => vars.Begin();

        public IForwardIterator<int> RBegin() => vars.RBegin();
        #endregion

        #region overriding
        public override bool Equals(object obj) =>
#if DEBUG
            throw new NotSupportedException($"do not boxing: {nameof(ExpressInt)}");
#else
            false;
#endif

        public override int GetHashCode() => vars.GetHashCode();

        public override string ToString() => vars.ToString(null, " ");

        #region compare
        public static bool operator ==(ExpressInt left, ExpressInt right) => left.vars == right.vars;
        public static bool operator !=(ExpressInt left, ExpressInt right) => left.vars != right.vars;

        public static bool operator ==(ExpressInt left, TokenType right) => left.vars[0] == (int)right;
        public static bool operator !=(ExpressInt left, TokenType right) => left.vars[0] != (int)right;

        public static bool operator ==(TokenType left, ExpressInt right) => (int)left == right.vars[0];
        public static bool operator !=(TokenType left, ExpressInt right) => (int)left == right.vars[0];

        public static bool operator ==(ExpressInt left, int right) => left.vars[0] == right;
        public static bool operator !=(ExpressInt left, int right) => left.vars[0] != right;

        public static bool operator ==(int left, ExpressInt right) => left == right.vars[0];
        public static bool operator !=(int left, ExpressInt right) => left == right.vars[0];
        #endregion
        #endregion

        #region implicit
        public static implicit operator ExpressInt(string str) => new ExpressInt(str, EnumTM.Default);
        public static implicit operator ExpressInt(TokenType[] items) => new ExpressInt(items);

        public static implicit operator int[](ExpressInt inst) => inst.vars;
        public static implicit operator int(ExpressInt inst) => inst.vars[0];
        #endregion
    }
}
