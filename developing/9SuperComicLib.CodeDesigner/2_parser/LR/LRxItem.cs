using System;
using System.Runtime.CompilerServices;
using SuperComicLib.Collections;
using SuperComicLib.Core;

namespace SuperComicLib.CodeDesigner
{
#if DEBUG
    
    internal sealed class LALRTableView
    {
        private readonly LRxItem item;

        public LALRTableView(LRxItem item) => this.item = item;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        public TokenType[] Items => Array.ConvertAll(item.lookahead.ToArray(), CONV);

        private static TokenType CONV(int x) => (TokenType)x;
    }
#endif

#if DEBUG
    [System.Diagnostics.DebuggerTypeProxy(typeof(LALRTableView))]
#endif
    public sealed class LRxItem : IEquatable<LRxItem>
    {
        public readonly int produce;
        public readonly ExpressInt express;
        public readonly int index;
        public readonly CHashSet<int> lookahead;

        public LRxItem(GItem source, CHashSet<int> la)
        {
            produce = source.produce;
            express = source.express;
            lookahead = la;
        }

        public LRxItem(int produce, ExpressInt express, int index, CHashSet<int> lookahead)
        {
            this.produce = produce;
            this.express = express;
            this.index = index;
            this.lookahead = lookahead;
        }

        public bool IsEnd => index >= express.Length;

        public bool IsLast => index + 1 == express.Length;

        public int MarkSymbol =>
            index < express.Length
            ? express[index]
            : -1;

        public LRxItem GetNext() =>
            new LRxItem(produce, express, index + 1, lookahead);

        public bool CoreEquals(LRxItem other) =>
            produce == other.produce &&
            express.Equals(other.express) &&
            index == other.index;

        public bool Equals(LRxItem other) =>
            CoreEquals(other) &&
            lookahead.SetEquals(other.lookahead);

        public override int GetHashCode()
        {
            int result = 7;
            IntHash.Combine(ref result, produce);
            IntHash.Combine(ref result, express.GetHashCode());
            IntHash.Combine(ref result, index);
            
            return result;
        }

        // public int GetCoreHashCode()
        // {
        //     int result = 3;
        //     IntHash.Combine(ref result, produce);
        //     IntHash.Combine(ref result, express.GetHashCode());
        // 
        //     return result;
        // }

#if DEBUG
        public override string ToString()
        {
            if (index == 0)
                return $"{Grammar.bag[produce]} -> . {express}";
            else if (IsEnd)
                return $"{Grammar.bag[produce]} -> {express} .";

            string st = $"{Grammar.bag[produce]} ->";

            ExpressInt e = express;
            int max = e.Length;
            for (int x = 0; x < max; x++)
            {
                if (x == index)
                    st += " .";

                int now = e[x];
                st += " " +
                    (now.IsTerminal()
                    ? ((TokenType)now).ToString()
                    : Grammar.bag[now]);
            }

            return st;
        }
#endif
    }
}
