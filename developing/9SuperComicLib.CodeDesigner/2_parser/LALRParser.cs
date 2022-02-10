using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public class LALRParser : ParserBase
    {
        public const int
            act_SHIFT = 1,
            act_REDUCE = 2,
            act_ACCEPT = 3;

        protected CachedGrammar m_cachedG;
        protected internal int threadLimit;
        protected internal Map<int>[] gtb;
        protected internal Map<TableItem>[] atb;
        protected internal bool readyToParse;

        #region constructor
        public LALRParser(Grammar grammar) : this(grammar, ExceptionHandlerFactory.Default, false, 1)
        {
        }

        public LALRParser(Grammar grammar, IExceptionHandler handler) : this(grammar, handler, false, 1)
        {
        }

        public LALRParser(Grammar grammar, IExceptionHandler handler, bool leaveOpen) : this(grammar, handler, leaveOpen, 1)
        {
        }

        public LALRParser(Grammar grammar, IExceptionHandler handler, bool leaveOpen, int threadLimit) : base(grammar, handler, leaveOpen)
        {
            this.threadLimit = threadLimit;
        }
        #endregion

        #region debug
#if DEBUG
        public void Setup() => OnParsePrepare();
#endif
        #endregion

        #region override
        protected override bool OnParsePrepare()
        {
            if (!readyToParse)
            {
                CachedGrammar g = new CachedGrammar(m_grammar);
                g.ParallelCaching(threadLimit);

                List<LALRState> states = CreateLALRState(g);
                gtb = CreateGOTO(states);
                atb = CreateATB(states);

                m_cachedG = g;
                readyToParse = true;
            }

            return true;
        }

        protected override INode OnParse(IEnumerator<Token> iterator, Stack<int> tempstack, Stack<INode> tempnodes)
        {
            tempstack.Push(0);

            Map<int>[] gtb = this.gtb;
            Map<TableItem>[] atb = this.atb;

            Grammar g = m_grammar;

            do
            {
                int current = (int)iterator.Current.type;
                if (!atb[tempstack.Peek()].TryGet(current, out TableItem ctb))
                {
#if DEBUG
                    var debug_state = tempstack.Peek();
                    var debug_table = atb[debug_state];

                    foreach (var key in debug_table.Keys)
                        System.Diagnostics.Debug.WriteLine(((TokenType)key).ToString());

                    var debug_item = iterator.Current;
                    System.Diagnostics.Debugger.Break();
#endif
                    OnParsingError(tempstack, iterator, current);
                    m_handler.Fail(FMSG.I);
                    return null;
                }

                int act = ctb.actType;
                if (act == act_SHIFT)
                {
                    tempnodes.Push(new TokNode(iterator.Current));
                    tempstack.Push(ctb.nextstate);

                    iterator.MoveNext();
                }
                else if (act == act_REDUCE)
                {
                    GItem item = g.m_items[ctb.nextstate];
                    int len = item.express.Length;

                    if (len > 1)
                    {
                        ExNode node = new ExNode(len);
                        while (--len >= 0)
                        {
                            tempstack.Pop();
                            node.Add(tempnodes.Pop());
                        }
                        tempnodes.Push(node);
                    }
                    else
                        tempstack.Pop();

                    tempstack.Push(gtb[tempstack.Peek()].Get(item.produce));
                }
                else if (act == act_ACCEPT)
                    return tempnodes.Pop();
            } while (true);
        }
        #endregion

        #region virtual method
        protected virtual void OnParsingError(Stack<int> tempstack, IEnumerator<Token> iterator, int current)
        {
        }
        #endregion

        #region method
        private List<LALRState> CreateCLRState(CachedGrammar cg)
        {
            LALRState now = new LALRState(new LRxItem(cg.m_value.StartItem, new CHashSet<int> { ExpressInt.end_symbol }));
            LR1Helper.Closure(cg, now.items);

            List<LALRState> list = new List<LALRState> { now };

            for (int x = 0; x < list.Count; x++)
            {
                now = list[x];

                IForwardIterator<LRxItem> iter = now.items.Begin();
                IForwardIterator<LRxItem> e1 = now.items.Begin();
                do
                {
                    if (iter.Value.IsEnd)
                        continue;

                    LinkedHashSet<LRxItem> nextit = new LinkedHashSet<LRxItem>();
                    int ms = iter.Value.MarkSymbol;
                    do
                    {
                        if (e1.Value.MarkSymbol == ms)
                            nextit.Add(e1.Value.GetNext());
                    } while (e1.LazyAdd());
                    e1.Reset();

                    LR1Helper.Closure(cg, nextit);

                    bool exist = false;
                    for (int z = 0; z < list.Count; z++)
                        if (list[z].items.SetEquals(nextit))
                        {
                            exist = true;
                            now.map.Add(ms, list[z]);
                        }

                    if (!exist)
                    {
                        LALRState next = new LALRState(nextit);
                        list.Add(next);
                        now.map.Add(ms, next);
                    }
                } while (iter.LazyAdd());

                iter.Dispose();
                e1.Dispose();
            }

            return list;
        }

        private List<LALRState> CreateLALRState(CachedGrammar cg)
        {
            List<LALRState> clr = CreateCLRState(cg);
            List<LALRState> lalr = new List<LALRState>();

            int cnt = clr.Count;

            for (int x = 0; x < cnt; x++)
            {
                LALRState current_state = clr[x];

                for (int z = x + 1; z < cnt;)
                {
                    LinkedHashSet<LRxItem> xlist = current_state.items;
                    LinkedHashSet<LRxItem> zlist = clr[z].items;

                    IEqualityComparer<LRxItem> save = xlist.Comparer;

                    xlist.Comparer = LRxItemComparerSlim.Instance;
                    zlist.Comparer = LRxItemComparerSlim.Instance;
                    if (xlist.SetEquals(zlist))
                    {
                        IEnumerator<LRxItem> e1 = xlist.GetEnumerator();
                        while (e1.MoveNext())
                        {
                            LRxItem now = e1.Current;
                            IEnumerator<LRxItem> e2 = zlist.GetEnumerator();
                            while (e2.MoveNext())
                                if (now.CoreEquals(e2.Current))
                                {
                                    now.lookahead.UnionWith(e2.Current.lookahead);
                                    break;
                                }
                            e2.Dispose();
                        }
                        e1.Dispose();

                        xlist.Comparer = save;
                        zlist.Comparer = save;
                        for (int a = 0; a < cnt; a++)
                        {
                            Map<LALRState> map = clr[a].map;
                            KeyValuePair<int, LALRState>[] kvs = map.ToArray();
                            for (int b = 0, bm = kvs.Length; b < bm; b++)
                                if (kvs[b].Value.items.SetEquals(zlist))
                                    map.Add(kvs[b].Key, current_state);
                        }

                        clr.RemoveAt(z);
                        cnt--;
                    }
                    else
                    {
                        xlist.Comparer = save;
                        zlist.Comparer = save;

                        z++;
                    }
                }

                lalr.Add(current_state);
            }

            clr.Clear();
            return lalr;
        }

        private Map<int>[] CreateGOTO(List<LALRState> state)
        {
            int cnt = state.Count;

            Map<int>[] res = new Map<int>[cnt];
            for (int x = 0; x < cnt; x++)
            {
                Map<int> item = new Map<int>();

                IEnumerator<KeyValuePair<int, LALRState>> e1 = state[x].map.GetEnumerator();
                while (e1.MoveNext())
                    if (!e1.Current.Key.IsTerminal())
                        item.Add(e1.Current.Key, IndexOf(state, e1.Current.Value));

                e1.Dispose();

                res[x] = item;
            }

            return res;
        }

        private Map<TableItem>[] CreateATB(List<LALRState> state)
        {
            Grammar g = m_grammar;

            int cnt = state.Count;
            int stnon = g.startIdx.ToNonterminal();

            Map<TableItem>[] res = new Map<TableItem>[cnt];
            int x = 0;
            for (; x < cnt; x++)
            {
                Map<TableItem> item = new Map<TableItem>();

                IEnumerator<KeyValuePair<int, LALRState>> e1 = state[x].map.GetEnumerator();
                while (e1.MoveNext())
                    if (e1.Current.Key.IsTerminal())
                        item.Add(e1.Current.Key, new TableItem(act_SHIFT, IndexOf(state, e1.Current.Value)));

                e1.Dispose();
                res[x] = item;
            }

            for (x = 0; x < cnt; x++)
            {
                IEnumerator<LRxItem> e1 = state[x].items.GetEnumerator();
                Map<TableItem> ctb = res[x];

                while (e1.MoveNext())
                {
                    LRxItem cit = e1.Current;
                    if (cit.IsEnd)
                        if (cit.produce == stnon)
                            ctb.Add(ExpressInt.end_symbol, new TableItem(act_ACCEPT, 0));
                        else
                        {
                            int index = IndexOf(g, cit);
                            TableItem reduce = new TableItem(act_REDUCE, index);

                            IEnumerator<int> e2 = cit.lookahead.GetEnumerator();
                            while (e2.MoveNext())
                                if (ctb.Contains(e2.Current))
                                {
#if DEBUG
                                    LALRState current_state = state[x];
                                    TokenType debug_tok = (TokenType)e2.Current;
#endif
                                    m_handler.Fail($"SHIFT-REDUCE CONFILCT -> state: {x}");
                                }
                                else
                                    ctb.Add(e2.Current, reduce);

                            e2.Dispose();
                        }
                }

                e1.Dispose();
            }

            return res;
        }

        private static int IndexOf(List<LALRState> state, LALRState item)
        {
            for (int x = 0; x < state.Count; x++)
                if (state[x].Has(item))
                    return x;

            return -1;
        }

        private static int IndexOf(Grammar g, LRxItem item)
        {
            ExpressInt express = item.express;

#if DEBUG
            int debug = item.produce.ToIndex();
            string debug_str = Grammar.bag[item.produce];
#endif
            Range rng = g.m_nonterminals[item.produce.ToIndex()];
            int st = rng.start;
            int ed = rng.end;
            do
            {
                if (g.m_items[st].express.Equals(express))
                    return st;
            } while (++st < ed);

            return -1;
        }
        #endregion

        #region disposable
        protected override void Dispose(bool disposing)
        {
            if (m_cachedG != null)
            {
                if (leaveOpen)
                    m_cachedG.m_value = null;

                m_cachedG.Dispose();
                m_cachedG = null;
            }

            int x;
            if (gtb != null)
            {
                x = gtb.Length;
                while (--x >= 0)
                {
                    gtb[x].Dispose();
                    gtb[x] = null;
                }

                gtb = null;
            }

            if (atb != null)
            {
                x = atb.Length;
                while (--x >= 0)
                {
                    atb[x].Dispose();
                    atb[x] = null;
                }

                atb = null;
            }

            threadLimit = 0;
            readyToParse = false;

            base.Dispose(disposing);
        }
        #endregion
    }
}
