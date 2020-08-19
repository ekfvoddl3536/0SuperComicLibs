using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class Grammar : IDisposable
    {
        public readonly int startNonterminal;
        private GItem[] m_items;
        private Map<Range> m_nonterminals;
        // private Map<int[]> m_first;
        // private Map<int[]> m_follow;

        public Grammar(StreamReader reader) : this(reader, null) { }

        public Grammar(StreamReader reader, IExpressMap map)
        {
            if (map == null)
                map = EnumTM.Default;

            List<GItem> its = new List<GItem>(32);
            Map<Range> ntm = new Map<Range>();

            while (!reader.EndOfStream)
            {
                string vtmp = reader.MoveNext();
                while (vtmp.Last() == '|') // more
                    vtmp += reader.MoveNext();

                string[] tmp = vtmp.Split('=');
                int produce = tmp[0].Trim().ToNonterminal();
                
                IForwardIterator<string> e1 = tmp[1].Trim().Split('|').Begin();

                if (its.Count == 0)
                {
                    startNonterminal = produce;
                    its.Add(new GItem(ExpressInt.start_symbol, new ExpressInt(e1.Value, map)));
                }

                int st = its.Count;
                do
                    its.Add(new GItem(produce, new ExpressInt(e1.Value, map)));
                while (e1.LazyAdd());

                ntm.Add(produce, new Range(st, its.Count - 1));
            }

            m_items = its.ToArray();
            m_nonterminals = ntm;
        }

        #region indexer
        public GItem this[int index] => m_items[index];
        #endregion

        #region first
        public int[] FIRST(int begin_nonterminal)
        {
            if (m_nonterminals.TryGet(begin_nonterminal, out Range rng))
            {
                var result = new HashSet<int>();
                GHelper.FirstAll(result, m_items, m_nonterminals, rng, begin_nonterminal);
                return result.ToArray();
            }

            return Array.Empty<int>();
        }

        public int[] FIRST(string begin_nonterminal) =>
            FIRST(begin_nonterminal.ToNonterminal());

        public int[] FIRST(int begin_nonterminal, int start_position)
        {
            if (m_nonterminals.TryGet(begin_nonterminal, out Range rng))
            {
                var result = new HashSet<int>();

                GItem[] items = m_items;
                IForwardIterator<int> iterator = items[rng.start].express.Begin();
                ((IIndexable)iterator).Position = start_position;

                GHelper.FirstAll(
                    result, 
                    new HashSet<int>(), 
                    items, 
                    m_nonterminals,
                    rng,
                    iterator);

                return result.ToArray();
            }

            return Array.Empty<int>();
        }

        public int[] FIRST(string begin_nonterminal, int start_position) =>
            FIRST(begin_nonterminal.ToNonterminal(), start_position);
        #endregion

        #region follow
        public int[] FOLLOW(int begin_nonterminal)
        {
            if (m_nonterminals.TryGet(begin_nonterminal, out Range rng))
            {
                var result = new HashSet<int>();
                GHelper.FollowAll(result, m_items, m_nonterminals, rng, begin_nonterminal);
                return result.ToArray();
            }

            return Array.Empty<int>();
        }

        public int[] FOLLOW(string begin_nonterminal) => FOLLOW(begin_nonterminal.ToNonterminal());

        public int[] FOLLOW(int begin_nonterminal, int start_position)
        {
            if (m_nonterminals.TryGet(begin_nonterminal, out Range rng))
            {
                var result = new HashSet<int>();

                GItem[] items = m_items;
                IForwardIterator<int> iterator = items[rng.start].express.Begin();
                ((IIndexable)iterator).Position = start_position;

                GHelper.FollowAll(
                    result, 
                    new HashSet<int>(), 
                    new Stack<int>(),
                    items, 
                    m_nonterminals, 
                    rng, 
                    iterator);

                return result.ToArray();
            }

            return Array.Empty<int>();
        }

        public int[] FOLLOW(string begin_nonterminal, int start_position) =>
            FIRST(begin_nonterminal.ToNonterminal(), start_position);
        #endregion

        #region finder
        public IEnumerable<GItem> Find(int nontermnial) =>
            m_nonterminals.TryGet(nontermnial, out Range rng) 
            ? m_items.Slice(rng.start, rng.end - 1)
            : null;
        #endregion

        #region get
        public Map<Range> GetNonTerminals() => m_nonterminals;

        public GItem[] GetGItems() => m_items;
        #endregion

        #region disposable
        public void Dispose()
        {
            if (m_items != null)
            {
                m_items = null;

                m_nonterminals.Dispose();
                m_nonterminals = null;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
