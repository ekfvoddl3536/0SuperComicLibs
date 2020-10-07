using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    internal static class LR1Helper
    {
        public static void Closure(CachedGrammar inst, LinkedHashSet<LRxItem> items)
        {
            Grammar g = inst.m_value;
            IForwardIterator<LRxItem> e1 = items.Begin();
            IForwardIterator<LRxItem> e2 = items.Begin();

            for (; e1.IsAlive; e1.Add())
            {
                LRxItem current = e1.Value;
                if (current.IsEnd)
                    continue;

                int ms = current.MarkSymbol;
                if (!ms.IsTerminal())
                {
                    CHashSet<int> la = 
                        current.IsLast 
                        ? new CHashSet<int>(current.lookahead) 
                        : inst.GetFirst(current.express, current.index + 1);

                    IForwardIterator<GItem> e3 = g.Find(ms);
                    do
                    {
                        LRxItem newit = new LRxItem(e3.Value, la);

                        for (; e2.IsAlive; e2.Add())
                            if (newit.CoreEquals(e2.Value))
                            {
                                CHashSet<int> ela = e2.Value.lookahead;
                                if (!ela.IsSupersetOf(la))
                                    ela.UnionWith(la);

                                goto exit;
                            }

                        items.Add(newit);

                    exit:
                        e2.Reset();
                    } while (e3.LazyAdd());
                    e3.Dispose();
                }
            }
            
            e1.Dispose();
        }
    }
}
