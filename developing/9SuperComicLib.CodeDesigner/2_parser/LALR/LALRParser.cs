using System;
using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public class LALRParser : ParserBase
    {
        protected const int
            act_SHIFT = 1,
            act_REDUCE = 2,
            act_ACCEPT = 3;

        #region fields
        protected List<LALRState> m_list;
        protected Map<int>[] m_gotoTable;
        protected Map<TableItem>[] m_actTable;
        protected CachedGrammar m_cachedG;

        protected int m_threadLimit;
        #endregion

        #region constructor
        public LALRParser(Grammar grammar) : this(grammar, ExceptionHandlerFactory.Default, 0)
        {
        }

        public LALRParser(Grammar grammar, IExceptionHandler handler) : this(grammar, handler, 0)
        {
        }

        public LALRParser(Grammar grammar, int threadLimit) : this(grammar, ExceptionHandler.Default, threadLimit)
        {
        }

        public LALRParser(Grammar grammar, IExceptionHandler handler, int threadLimit) : base(grammar, handler) =>
            m_threadLimit = threadLimit.RngIn(0, Environment.ProcessorCount);
        #endregion

        protected override bool OnParsePrepare()
        {
            if (m_cachedG == null)
            {
                CachedGrammar g = new CachedGrammar(m_grammar);
                g.ParallelCaching(m_threadLimit);

                OnCreateLALRStates(g);
                OnCreateGOTO();
                OnCreateACTION(m_grammar);

                m_cachedG = g;
            }

            return m_handler.FailCount == 0;
        }

        #region lalr
        protected void OnCreateLALRStates(CachedGrammar g)
        {
            OnCreateCLRStates(g);
            List<LALRState> item = new List<LALRState>();
            ref List<LALRState> list = ref m_list;

            for (int x = 0; x < list.Count; x++)
            {
                LALRState state = list[x];
                for (int z = x + 1; z < list.Count;)
                    if (state.items.SetEquals(list[z].items))
                    {
                        LALRState state2 = list[z];
                        foreach (LALRGItem i1 in state.items)
                            foreach (LALRGItem i2 in state2.items)
                                if (i1.Equals((GItem)i2))
                                {
                                    i1.lookahead.AddRange(i2.lookahead);
                                    break;
                                }

                        // wtf troll-code?
                        // for (int a = 0; a < list.Count; a++)
                        // {
                        //     Map<LALRState> map = list[a].map;
                        //     foreach (int key in map.Keys)
                        //         if (map.Get(key).items.SetEquals(state2.items))
                        //         {
                        //             map.Add(key, )
                        //         }
                        // }

                        list.RemoveAt(z);
                    }
                    else
                        z++;

                item.Add(state);
            }

            list.Clear();
            list = item;
        }

        protected void OnCreateCLRStates(CachedGrammar cg)
        {
            var vs = new List<LALRState>
            {
                new LALRState(cg, new LinkedHashSet<LALRGItem>
                {
                    new LALRGItem(cg.Value[0])
                })
            };

            var currents = new Map<UserLinkedValue<LALRGItem>>();
            for (int x = 0; x < vs.Count; x++)
            {
                LALRState state = vs[x];
                IEnumerator<LALRGItem> e1 = state.items.GetEnumerator();
                LALRGItem item;
                while (e1.MoveNext())
                {
                    if (e1.Current.IsEnd)
                        continue;

                    item = e1.Current;
                    if (currents.TryGet(item.MarkSymbol, out UserLinkedValue<LALRGItem> ulv))
                        ulv.AddLast(new UserLinkedValue<LALRGItem>(item.MoveNext()));
                    else
                    {
                        ulv = new UserLinkedValue<LALRGItem>(item.MoveNext());
                        ulv.SetHEAD();

                        currents.Add(item.MarkSymbol, ulv);
                    }
                }

                foreach (int key in currents.Keys)
                {
                    LinkedHashSet<LALRGItem> nextitems = new LinkedHashSet<LALRGItem>();
                    nextitems.AddRange(currents.Get(key).ToArrayFast());

                    LALRState nextstate = new LALRState(cg, nextitems);

                    bool found = false;
                    for (int z = 0; z < vs.Count; z++)
                        if (vs[z].items.SetEquals(nextstate.items))
                        {
                            found = true;
                            state.map.Add(key, vs[z]);
                        }

                    if (!found)
                    {
                        vs.Add(nextstate);
                        state.map.Add(key, nextstate);
                    }
                }

                e1.Dispose();

                var iterator = currents.Begin();
                for (; iterator.IsAlive; iterator.Add())
                    iterator.Value.DisconnectAll();

                iterator.Dispose();
                currents.Clear();
            }

            m_list = vs;
        }

        protected void OnCreateGOTO()
        {
            List<LALRState> list = m_list;
            int cnt = list.Count;

            Map<int>[] table = new Map<int>[cnt];

            while (--cnt >= 0)
            {
                Map<int> temp = new Map<int>();
                Map<LALRState> map = list[cnt].map;
                foreach (int k in map.Keys)
                    if (k < 0) // non-terminal
                        temp.Add(k, IndexOf(map.Get(k)));

                table[cnt] = temp;
            }

            m_gotoTable = table;
        }

        protected int IndexOf(LALRState state)
        {
            List<LALRState> list = m_list;
            for (int x = 0; x < list.Count; x++)
                if (list[x].Equals(state))
                    return x;

            return -1;
        }

        protected void OnCreateACTION(Grammar grammar)
        {
            List<LALRState> list = m_list;
            int cnt = list.Count;

            Map<TableItem>[] table = new Map<TableItem>[cnt];

            int x = cnt;
            while (--x >= 0)
            {
                Map<TableItem> temp = new Map<TableItem>();

                // 직접 관리
                IEnumerator<LALRGItem> e1 = list[x].items.GetEnumerator();
                IEnumerator<int> e2 = list[x].map.Keys.GetEnumerator();
                while (e1.MoveNext())
                {
                    LALRGItem item = e1.Current;
                    int k;
                    if (item.index == item.express.Length)
                        if (item.index == 1 && item.express[0] == ExpressInt.start_symbol)
                            temp.Add(ExpressInt.start_symbol, new TableItem(act_ACCEPT, 0));
                        else
                        {
                            e2 = item.lookahead.GetEnumerator();
                            while (e2.MoveNext())
                            {
                                k = e2.Current;
                                if (temp.Contains(k))
                                {
                                    m_handler.Fail(string.Format(FMSG.F2, temp.Get(k).ToString(), x.ToString()));
                                    e1.Dispose();
                                    e2.Dispose();
                                    return;
                                }
                                else
                                    temp.Add(k, new TableItem(act_REDUCE, grammar.GetGItems().FirstEqualsIndex(item)));
                            }
                            e2.Reset();
                        }
                    else
                    {
                        
                        while (e2.MoveNext())
                        {
                            k = e2.Current;
                            if (k.IsTerminal()) // terminal
                            {
                                if (temp.Contains(k))
                                {

                                }
                                temp.Add(k, new TableItem(act_SHIFT, IndexOf(list[x].map.Get(k))));
                            }
                        }
                        e2.Reset();
                    }
                }
                e1.Dispose();
                e2.Dispose();

                table[x] = temp;
            }

            m_actTable = table;
        }
        #endregion

        #region override
        protected override INode OnParse(IForwardIterator<Token> iterator, Stack<int> tempstack, Stack<INode> tempNodes)
        {
            Map<int>[] gtb = m_gotoTable;
            Map<TableItem>[] atb = m_actTable;
            Grammar g = m_grammar;

            tempstack.Push(0);
            for (; iterator.IsAlive; iterator.Add())
            {
                int current = (int)iterator.Value.type;
                // TableItem item = ;
                if (!atb[tempstack.Peek()].TryGet(current, out TableItem item))
                {   // Error
                    m_handler.Fail(FMSG.I + iterator.Value.ToString());
                    break;
                }

                int temp = item.actType;
                if (temp == act_ACCEPT)
                {
                    ExNode last = new ExNode(tempNodes.Count);
                    while (tempNodes.Count > 0)
                        last.Add(tempNodes.Pop());

                    return last;
                }
                else if (temp == act_SHIFT)
                {
                    tempNodes.Push(new TokNode(iterator.Value));

                    tempstack.Push(current);
                    tempstack.Push(item.nextstate);
                }
                else if (temp == act_REDUCE)
                {
                    GItem git = g[item.nextstate];
                    temp = git.express.Length;
                  
                    INode exnode = new ExNode(temp);
                    while (--temp >= 0)
                    {
                        exnode.Add(tempNodes.Pop());
                        tempstack.Pop();
                        tempstack.Pop();
                    }
                  
                    tempNodes.Push(exnode);
                  
                    current = tempstack.Peek();
                    temp = git.produce;

                    tempstack.Push(temp);
                    tempstack.Push(gtb[current].Get(temp));
                }
            }

            return
                tempNodes.Count == 1 
                ? tempNodes.Pop() 
                : null;
        }
        #endregion
    }
}
