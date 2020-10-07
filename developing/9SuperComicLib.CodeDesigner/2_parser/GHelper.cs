using System.Collections.Generic;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    internal static class GHelper
    {
        private readonly struct Item
        {
            public readonly int start;
            public readonly int end;
            public readonly int index;

            public Item(int start, int end, int index)
            {
                this.start = start;
                this.end = end;
                this.index = index;
            }
        }

        public static HashSet<int> FIRST(Grammar grammar, Range rng)
        {
            HashSet<int> result = new HashSet<int>();
            HashSet<int> done = new HashSet<int> { rng.start };

            First2(grammar, result, done, rng);

            done.Clear();
            return result;
        }

        private static void First2(Grammar grammar, HashSet<int> result, HashSet<int> done, Range rng)
        {
            Stack<Range> stack = new Stack<Range>();

        loop1:
            int st = rng.start;

        loop2:
            ExpressInt express = grammar.m_items[st].express;

            int now = express[0];
            if (now.IsTerminal())
                result.Add(now);
            else if (done.Add(now))
            {
                stack.Push(new Range(st + 1, rng.end));
                rng = grammar.m_nonterminals[now.ToIndex()];
                goto loop1;
            }

            if (++st < rng.end)
                goto loop2;

            while (stack.Count > 0)
            {
                rng = stack.Pop();
                if (rng.start >= rng.end)
                    continue;

                goto loop1;
            }
        }

        public static HashSet<int> FOLLOW(Grammar grammar, Range rng)
        {
            HashSet<int> result = new HashSet<int>();
            HashSet<int> done = new HashSet<int> { rng.start };

            Follow2(grammar, result, done, new Item(rng.start, rng.end, 0));
            // follow의 마지막은 항상 $
            result.Add(ExpressInt.end_symbol);

            done.Clear();
            return result;
        }

        private static void Follow2(Grammar grammar, HashSet<int> result, HashSet<int> done, Item item)
        {
            Stack<Item> its = new Stack<Item>();
            HashSet<int> firstdone = new HashSet<int>();

        loop1:
            int idx = item.index;
            int st = item.start;

        loop2:
            ExpressInt express = grammar.m_items[st].express;
            int len = express.Length;
            int mlen = len - 1;
            if (idx >= len)
            {
                its.Clear();
                return;
            }

        loop3:
            Range rng;
            int now = express[idx];
            if (!now.IsTerminal())
                if (idx < mlen)
                {
                    now = express[++idx];
                    if (now.IsTerminal())
                        result.Add(now);
                    else if (done.Add(now))
                    {
                        First2(grammar, result, firstdone, grammar.m_nonterminals[now.ToIndex()]);
                        firstdone.Clear();
                    }
                }
                else if (len == 1 && done.Add(now))
                {
                    its.Push(new Item(st, item.end, idx));

                    rng = grammar.m_nonterminals[now.ToIndex()];
                    item = new Item(rng.start, rng.end, 0);

                    goto loop1;
                }

            if (++idx < len)
                goto loop3;
            else if (++st < item.end)
            {
                idx = 0;
                goto loop2;
            }
            else if (its.Count > 0)
            {
                item = its.Pop();
                goto loop1;
            }
        }
    }
}
