using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class CachedGrammar : IDisposable
    {
        internal Grammar m_value;
        private Map<HashSet<int>> first;
        private Map<HashSet<int>> follow;

        public CachedGrammar(Grammar grammar) => m_value = grammar;

        #region property
        public Map<HashSet<int>> FirstSet => first;

        public Map<HashSet<int>> FollowSet => follow;

        public Grammar Source => m_value;
        #endregion

        #region methods
        public void Caching()
        {
            Grammar g = m_value;

            Range[] map = g.m_nonterminals;
            int cnt = map.Length;

            ref Map<HashSet<int>> first = ref this.first;
            ref Map<HashSet<int>> follow = ref this.follow;

            first = new Map<HashSet<int>>(cnt);
            follow = new Map<HashSet<int>>(cnt);
            follow.Add(ExpressInt.end_symbol, new HashSet<int>());

            for (int x = 0; x < cnt; x++)
            {
                Range rng = map[x];

                int idx = x.ToNonterminal();
                first.Add(idx, GHelper.FIRST(g, rng));
                follow.Add(idx, GHelper.FOLLOW(g, rng));
            }
        }

        public void ParallelCaching(int maxThreads)
        {
            if (maxThreads < 2)
            {
                Caching();
                return;
            }

            Grammar grammar = m_value;

            Range[] map = grammar.m_nonterminals;
            int cnt = map.Length;

            ref Map<HashSet<int>> first = ref this.first;
            ref Map<HashSet<int>> follow = ref this.follow;

            first = new Map<HashSet<int>>(cnt);
            follow = new Map<HashSet<int>>(cnt);
            follow.Add(ExpressInt.end_symbol, new HashSet<int>());

            Semaphore limit = new Semaphore(++maxThreads, maxThreads);
            AutoResetEvent are = new AutoResetEvent(true);

            // set
            Package.first = first;
            Package.follow = follow;
            Package.grammar = grammar;
            Package.limit = limit;
            Package.are = are;

            Task[] tasks = new Task[cnt];
            int x = cnt;
            for (int z = 0; z < cnt; z++)
            {
                limit.WaitOne();
                tasks[--x] = Task.Factory.StartNew(ThreadWorkCaching, new Package(z.ToNonterminal(), map[z]));
                limit.Release();
            }

            while (--cnt >= 0)
            {
                ref Task t = ref tasks[cnt];
                if (!t.IsCompleted)
                    t.Wait();

                t.Dispose();
                t = null;
            }

            // first.Add(ExpressInt.end_symbol, first.Get(m_value.startIdx));

            // clean-up
            Package.first = null;
            Package.follow = null;
            Package.grammar = null;
            Package.limit = null;
            Package.are = null;

            limit.Dispose();
            are.Dispose();
        }

        public CHashSet<int> GetFirst(ExpressInt express, int index)
        {
            CHashSet<int> result = new CHashSet<int>();
            if (index < express.Length)
                GetFirst(result, express, index);

            return result;
        }

        internal void GetFirst(CHashSet<int> set, ExpressInt expr, int idx)
        {
            int temp = expr[idx];
            if (temp.IsTerminal())
                set.Add(temp);
            else
                set.UnionWith(first.Get(temp));
        }

        public CHashSet<int> GetFollow(ExpressInt express, int index)
        {
            CHashSet<int> result = new CHashSet<int>();
            if (index < express.Length)
                GetFollow(result, express, index);

            return result;
        }

        internal void GetFollow(CHashSet<int> set, ExpressInt expr, int idx)
        {
            int temp = expr[idx];
            if (temp.IsTerminal())
                set.Add(temp);
            else
                set.UnionWith(follow.Get(temp));

            set.Add(ExpressInt.end_symbol);
        }

        public void Dispose()
        {
            if (m_value != null)
            {
                m_value.Dispose();
                m_value = null;
            }

            if (first != null)
            {
                first.Dispose();
                first = null;

                follow.Dispose();
                follow = null;
            }

            GC.SuppressFinalize(this);
        }
        #endregion

        #region parallel helpe
        private sealed class Package
        {
            public static Map<HashSet<int>> first;
            public static Map<HashSet<int>> follow;
            public static Grammar grammar;
            public static Semaphore limit;
            public static AutoResetEvent are;

            public readonly int nonterminal;
            public readonly Range rng;

            public Package(int nonterminal, Range rng)
            {
                this.nonterminal = nonterminal;
                this.rng = rng;
            }
        }

        private static void ThreadWorkCaching(object state)
        {
            var package = (Package)state;
            Semaphore limit = Package.limit;
            limit.WaitOne();

            Grammar grammar = Package.grammar;

            Range rng = package.rng;

            HashSet<int> set = GHelper.FIRST(grammar, rng);
            HashSet<int> set2 = GHelper.FOLLOW(grammar, rng);

            // sync block
            Package.are.WaitOne();

            // body
            int nonterminal = package.nonterminal;
            Package.first.Add(nonterminal, set);
            Package.follow.Add(nonterminal, set2);

            // exit
            Package.are.Set();

            limit.Release();
        }
        #endregion
    }
}
