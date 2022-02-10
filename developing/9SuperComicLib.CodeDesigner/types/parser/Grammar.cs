using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class Grammar : IDisposable
    {
#if DEBUG
        public static readonly Dictionary<int, string> bag = new Dictionary<int, string>();
#endif
        internal GItem[] m_items;
        internal Range[] m_nonterminals;
        internal int startIdx;

        #region construct
        internal Grammar() { }

        public Grammar(StreamReader reader) : this(reader, null) { }

        public Grammar(StreamReader reader, ISymbolMap map)
        {
            if (map == null)
                map = new EnumTM();

            List<GItem> its = new List<GItem>(64);
            List<Range> ntm = new List<Range>(64);

            int x;

            map.BeginParseGrammar(ntm);

        loop:
            while (!reader.EndOfStream)
            {
                string vtmp = reader.MoveNext();
                if (vtmp.Length == 0 || vtmp[0] == '#') // comment
                    goto loop;

                while (vtmp.Last() == '|') // more
                    vtmp += reader.MoveNext();

                string[] tmp = vtmp.Split(Constants.eqs, 2);
                string left = tmp[0].Trim();

                if (map.ContainsNT(left))
                    throw new InvalidOperationException("invalid grammar: already exist non-terminal symbol -> " + tmp[0].Trim());

                IForwardIterator<string> e1 = tmp[1].Trim().SplitExceptRange('|', '\'', '\'').Begin();

                int produce = map.NonTerminal(left);
                x = its.Count;
                do
                    its.Add(new GItem(produce, new ExpressInt(e1.Value, map)));
                while (e1.LazyAdd());

                ntm[produce.ToIndex()] = new Range(x, its.Count);
#if DEBUG
                bag.Add(produce, tmp[0].Trim());
#endif
            }
            
            if (map.ContainsNT("S"))
            {
                int idx = map.NonTerminal("S").ToIndex();
                Range rng = ntm[idx];
                if (rng.end - rng.start == 1) // solo
                    startIdx = idx;
                else
                {
                    x = its.Count;

                    int newstart = ntm.Count;
                    its.Add(new GItem(newstart, new ExpressInt(idx.ToNonterminal())));
                    ntm.Add(new Range(x, x));
                    startIdx = newstart;
#if DEBUG
                    bag.Add(newstart, "S'");
#endif
                }
            }
            else
                throw new InvalidOperationException("invalid grammar: start nonterminal");

            map.EndParseGrammar();

            m_items = its.ToArray();
            m_nonterminals = ntm.ToArray();
        }
        #endregion

        #region finder
        public IForwardIterator<GItem> Find(int nontermnial)
        {
            Range rng = m_nonterminals[nontermnial.ToIndex()];
            return m_items.Slice(rng.start, rng.end).Begin();
        }

        public GItem StartItem => m_items[m_nonterminals[startIdx].start];
        #endregion

        #region method
        public bool IsVaild(ISymbolMap symbol, out string message)
        {
            DebugSymbolMap debug = symbol as DebugSymbolMap;

            GItem[] vs = m_items;
            Range[] map = m_nonterminals;

            int x = vs.Length;

            string result = string.Empty;
            int errorcnt = 1;
            while (--x >= 0)
            {
                GItem current = vs[x];
                
                ExpressInt expression = current.express;
                int len = expression.Length;

                while (--len >= 0)
                {
                    int now = expression[len];
                    if (!now.IsTerminal() && map[now.ToIndex()] == default)
                        result += $"[{errorcnt++}] Invalid Non-Terminal: " + (debug?.Get(now) ?? $"<{now}>") + Environment.NewLine;
                }
            }

            message = result;
            return result.Length == 0;
        }
        #endregion

        #region disposable
        public void Dispose()
        {
            if (m_items != null)
            {
                m_items = null;
                m_nonterminals = null;
                startIdx = 0;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
