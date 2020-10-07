using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class LALRState
    {
        public LinkedHashSet<LRxItem> items;
        public Map<LALRState> map = new Map<LALRState>();

        public LALRState(LRxItem first) => items = new LinkedHashSet<LRxItem> { first };

        public LALRState(LinkedHashSet<LRxItem> items) => this.items = items;

        public bool Has(LALRState other)
        {
            if (!map.SetEquals(other.map) ||
                items.Count != other.items.Count)
                return false;

            IEnumerator<LRxItem> me = items.GetEnumerator();
            IEnumerator<LRxItem> e1 = other.items.GetEnumerator();

            while (me.MoveNext() && e1.MoveNext())
            {
                LRxItem mc = me.Current;
                LRxItem ec = e1.Current;

                if (!mc.CoreEquals(ec) ||
                    !mc.lookahead.IsSupersetOf(ec.lookahead))
                {
                    me.Dispose();
                    e1.Dispose();

                    return false;
                }
            }
            me.Dispose();
            e1.Dispose();

            return true;
        }

#if DEBUG
        public override string ToString() =>
            $"{items.Count}, {map.Count} -> {items.First.Value}";
#endif
    }
}
