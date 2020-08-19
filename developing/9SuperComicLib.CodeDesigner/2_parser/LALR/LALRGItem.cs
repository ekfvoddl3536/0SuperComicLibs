using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public sealed class LALRGItem : GItem, IEquatable<LALRGItem>
    {
        public int index;
        public readonly HashSet<int> lookahead;

        public LALRGItem(GItem src) : base(src.produce, src.express) =>
            lookahead = new HashSet<int> { ExpressInt.start_symbol };

        public LALRGItem(GItem src, HashSet<int> lookahead) : base(src.produce, src.express)
        {
            index = src.express.IsEpsilon.ToInt();
            this.lookahead = lookahead;
        }

        public LALRGItem(int produce, ExpressInt express, HashSet<int> lookahead, int index) : base(produce, express)
        {
            this.index = index;
            this.lookahead = lookahead;
        }

        public bool IsLastElement => express.Length - 1 == index;

        public bool IsEnd => express.Length <= index;

        public int MarkSymbol =>
            index >= express.Length
            ? -1
            : express[index];

        public LALRGItem MoveNext() => new LALRGItem(produce, express, lookahead, index + 1);

        public bool Equals(LALRGItem other) =>
            Equals((GItem)other) && index == other.index;
        
    }
}
