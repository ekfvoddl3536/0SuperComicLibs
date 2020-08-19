using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class CachedGrammar : IDisposable
    {
        private Grammar value;
        private Map<HashSet<int>> first;
        private Map<HashSet<int>> follow;

        public CachedGrammar(Grammar grammar) => value = grammar;

        #region property
        public Grammar Value => value;

        public Map<HashSet<int>> FirstSet => first;

        public Map<HashSet<int>> FollowSet => follow;
        #endregion

        #region methods
        public void Caching()
        {
            Map<Range> map = value.GetNonTerminals();
            int cnt = map.Count;

            ref Map<HashSet<int>> first = ref this.first;
            ref Map<HashSet<int>> follow = ref this.follow;

            first = new Map<HashSet<int>>(cnt);
            follow = new Map<HashSet<int>>(cnt);
            follow.Add(ExpressInt.start_symbol, new HashSet<int>());

            IEnumerator<KeyValuePair<int, Range>> e = map.GetEnumerator();
            GItem[] vs = value.GetGItems();

            while (e.MoveNext())
            {
                KeyValuePair<int, Range> temp = e.Current;
                int nonterminal = temp.Key;
                Range rng = temp.Value;

                HashSet<int> set = new HashSet<int>();
                GHelper.FirstAll(set, vs, map, rng, nonterminal);
                first.Add(nonterminal, set);

                set = new HashSet<int>();
                GHelper.FollowAll(set, vs, map, rng, nonterminal);
                follow.Add(nonterminal, set);
            }

            first.Add(ExpressInt.start_symbol, first.Get(value.startNonterminal));
            e.Dispose();
        }

        public void ParallelCaching(int maxThreads)
        {
            if (maxThreads < 2)
            {
                Caching();
                return;
            }

            Map<Range> map = value.GetNonTerminals();
            int cnt = map.Count;

            ref Map<HashSet<int>> first = ref this.first;
            ref Map<HashSet<int>> follow = ref this.follow;

            first = new Map<HashSet<int>>(cnt);
            follow = new Map<HashSet<int>>(cnt);
            follow.Add(ExpressInt.start_symbol, new HashSet<int>());

            IEnumerator<KeyValuePair<int, Range>> e = map.GetEnumerator();
            GItem[] vs = value.GetGItems();

            Semaphore limit = new Semaphore(++maxThreads, maxThreads);
            AutoResetEvent are = new AutoResetEvent(true);

            // set
            Package.first = first;
            Package.follow = follow;
            Package.map = map;
            Package.vs = vs;
            Package.limit = limit;
            Package.are = are;

            Task[] tasks = new Task[cnt];
            int x = cnt;
            while (e.MoveNext())
            {
                limit.WaitOne();
                tasks[--x] = Task.Factory.StartNew(ThreadWorkCaching, new Package(e.Current));
                limit.Release();
            }
            e.Dispose();

            while (--cnt >= 0)
            {
                ref Task t = ref tasks[cnt];
                if (!t.IsCompleted)
                    t.Wait();

                t.Dispose();
                t = null;
            }

            first.Add(ExpressInt.start_symbol, first.Get(value.startNonterminal));

            // clean-up
            Package.first = null;
            Package.follow = null;
            Package.map = null;
            Package.vs = null;
            Package.limit = null;
            Package.are = null;

            limit.Dispose();
            are.Dispose();
        }

        public HashSet<int> GetFirst(ExpressInt express, int index)
        {
            HashSet<int> result = new HashSet<int>();
            GetFirst(result, express, index);
            return result;
        }

        public void GetFirst(HashSet<int> set, ExpressInt expr, int idx)
        {
            do
            {
                if (idx >= expr.Length)
                    return;

                int temp = expr[idx];
                if (temp == ExpressInt.epsilon)
                    continue;
                else if (temp.IsTerminal())
                    set.Add(temp);
                else // if (value.GetNonTerminals().Contains(temp))
                    set.AddRange(first.Get(temp));
            }
            while (++idx != expr.Length);
        }

        public void Dispose()
        {
            value.Dispose();
            value = null;

            first.Dispose();
            first = null;

            follow.Dispose();
            follow = null;

            GC.SuppressFinalize(this);
        }
        #endregion

        #region parallel helpe
        private sealed class Package
        {
            public static Map<HashSet<int>> first;
            public static Map<HashSet<int>> follow;
            public static Map<Range> map;
            public static GItem[] vs;
            public static Semaphore limit;
            public static AutoResetEvent are;

            public readonly int nonterminal;
            public readonly Range rng;

            public Package(KeyValuePair<int, Range> kv)
            {
                nonterminal = kv.Key;
                rng = kv.Value;
            }
        }

        private static void ThreadWorkCaching(object state)
        {
            var package = (Package)state;
            Semaphore limit = Package.limit;
            limit.WaitOne();

            Map<Range> map = Package.map;
            GItem[] vs = Package.vs;

            int nonterminal = package.nonterminal;
            Range rng = package.rng;

            HashSet<int> set = new HashSet<int>();
            GHelper.FirstAll(set, vs, map, rng, nonterminal);

            HashSet<int> set2 = new HashSet<int>();
            GHelper.FollowAll(set2, vs, map, rng, nonterminal);

            // sync block
            Package.are.WaitOne();
            // body
            Package.first.Add(nonterminal, set);
            Package.follow.Add(nonterminal, set2);
            // exit
            Package.are.Set();

            limit.Release();
        }
        #endregion
    }
}
