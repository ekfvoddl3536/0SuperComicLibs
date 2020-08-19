using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    internal static class GHelper
    {
        public static void FirstAll(HashSet<int> result, GItem[] items, Map<Range> map, Range rng, int begin_nonterminal)
        {
            HashSet<int> done = new HashSet<int> { begin_nonterminal };
            FirstAll(result, done, items, map, rng, null);

            // clean-up
            done.Clear();
        }

        public static void FirstAll(
            HashSet<int> result, 
            HashSet<int> done,
            GItem[] items, 
            Map<Range> map, 
            Range rng, 
            IForwardIterator<int> iterator)
        {
        loop:
            int st = rng.start, ed = rng.end;
            for (; st <= ed; st++)
            {
                if (iterator == null)
                    iterator = items[st].express.Begin();

                while (iterator.IsAlive)
                {
                    int v = iterator.Value;
                    iterator.Add();

                    if (v < 0 && v != ExpressInt.epsilon)
                    {
                        if (!done.Add(v)) // 이미 탐색하고 있거나 했음
                            continue;

                        // rngs.Push(new Range(st, ed));
                        // itrs.Push(e1);

                        rng = map.Get(v);

                        iterator.Dispose();
                        iterator = null;

                        goto loop;
                    }
                    else
                    {
                        result.Add(v);
                        break;
                    }
                }

                iterator.Dispose();
                iterator = null;
            }
        }

        public static void FollowAll(HashSet<int> result, GItem[] items, Map<Range> map, Range rng, int begin_nonterminal)
        {
            HashSet<int> done = new HashSet<int> { begin_nonterminal };
            Stack<int> nts = new Stack<int>();
            FollowAll(result, done, nts, items, map, rng, null);

            // clean-up
            done.Clear();
        }

        public static void FollowAll(
            HashSet<int> result,
            HashSet<int> done,
            Stack<int> stack,
            GItem[] items,
            Map<Range> map,
            Range rng,
            IForwardIterator<int> iterator)
        {
        loop:
            int st = rng.start, ed = rng.end;
            for (; st <= ed && done.Add(st); st++)
            {
                if (iterator == null)
                    iterator = items[st].express.Begin();

                int current = 0;
                while (iterator.IsAlive)
                {
                    int v = iterator.Value;
                    iterator.Add();

                    if (v == ExpressInt.epsilon)
                        continue;

                    if (v < 0) // non-terminal
                    {
                        current = v;
                        if (!done.Add(v)) // 이미 탐색하고 있거나 했음
                            continue;

                        stack.Push(v);
                    }
                    else if (current < 0) // setted
                    {
                        result.Add(v);
                        break;
                    }
                }

                iterator.Dispose();
                iterator = null;
            }

            if (stack.Count == 0)
                return;

            rng = map.Get(stack.Pop());
            goto loop;
        }
    }
}
