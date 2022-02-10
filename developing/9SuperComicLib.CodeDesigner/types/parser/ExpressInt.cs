using System;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    // A B C
    public readonly struct ExpressInt : IEquatable<ExpressInt>, IIterable<int>
    {
        #region constants
        public const int nonterminal_flag = 0x4000_0000;
        public const int nonterminal_bitmask = nonterminal_flag - 1;
        public const int end_symbol = 0x2000_0000;
        // public const int epsilon = 0x3F00_0000;
        #endregion

        #region static
        private static readonly char[] space = { ' ' };
        #endregion

        private readonly int[] vars;

        #region constructor
        public ExpressInt(int newitem) => vars = new int[1] { newitem };

        public ExpressInt(int[] items) => vars = items;

        public ExpressInt(TokenType[] items)
        {
            int x = items.Length;
            int[] vs = new int[x];

            while (--x >= 0)
                vs[x] = (int)items[x];

            vars = vs;
        }

        public ExpressInt(string str, ISymbolMap em) : this(new GString(str), em) { }

        public ExpressInt(GString str, ISymbolMap em)
        {
            int x = str.GetGroupCount();

            int[] vs = new int[x];

            for (int k = 0; str.MoveNext();)
                vs[k++] =
                    em.Terminal(str.Current, out TokenType result)
                    ? (int)result
                    : em.NonTerminal(str.Current);

            str.Dispose();
            vars = vs;
        }
        #endregion

        #region method && property
        public int Length => vars.Length;

        public int this[int index] => vars[index];

        public bool Equals(ExpressInt other)
        {
            int[] me = vars;
            int[] ot = other.vars;

            int x = me.Length;
            if (x != ot.Length)
                return false;

            while (--x >= 0)
                if (me[x] != ot[x])
                    return false;

            return true;
        }

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

        public override int GetHashCode() => Arrays.HashCodeFast(vars);

        public override string ToString()
#if DEBUG
        {
            int[] vs = vars;
            int now = vs[0];
            string result = 
                now.IsTerminal() 
                ? ((TokenType)now).ToString() 
                : Grammar.bag[now];

            int x = 1;
            int m = vs.Length;

            for (; x < m; x++)
            {
                now = vs[x];
                result += " " + 
                    (now.IsTerminal()
                    ? ((TokenType)now).ToString()
                    : Grammar.bag[now]);
            }


            return result;
        }
#else
            => string.Empty;
#endif
    }
    #endregion
}
