using System;
using System.Collections.Generic;
using System.Linq;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public class LALRState : IEquatable<LALRState>
    {
        public readonly LinkedHashSet<LALRGItem> items;
        public readonly Map<LALRState> map;

        public LALRState(LinkedHashSet<LALRGItem> items)
        {
            this.items = items;
            map = new Map<LALRState>();
        }

        public LALRState(CachedGrammar grammar, LinkedHashSet<LALRGItem> items)
        {
            this.items = items;
            map = new Map<LALRState>();

            Closure(grammar, items);
        }

        public bool Equals(LALRState other) =>
            items.SetEquals(other.items) &&
            map.SequenceEqual(other.map);

        public static void Closure(CachedGrammar grammar, LinkedHashSet<LALRGItem> items)
        {
            // LinkedHashSet<LALRGItem> items = this.items;

            IForwardIterator<LALRGItem> e1 = items.Begin();
            IForwardIterator<LALRGItem> e2 = items.Begin();

            for (; e1.IsAlive; e1.Add())
            {
                LALRGItem git = e1.Value;
                int current = git.MarkSymbol;
                if (current >= 0 && current.IsTerminal() == false)
                {
                    HashSet<int> lookahead;
                    if (git.IsLastElement)
                        lookahead = git.lookahead;
                    else
                    {
                        lookahead = grammar.GetFirst(git.express, git.index + 1);
                        lookahead.AddRange(git.lookahead);
                    }

                    // LALRGItem next = new LALRGItem()
                    bool found = false;
                    for (; e2.IsAlive; e2.Add())
                        if (e2.Value.produce == current) // existed
                        {
                            found = true;
                            e2.Value.lookahead.AddRange(lookahead);
                        }
                    e2.Reset();
                    
                    if (!found)
                    {
                        foreach (GItem src in grammar.Value.Find(current))
                            items.Add(new LALRGItem(src, lookahead));
                    }
                }
            }

            // LinkedNode<LALRGItem> head = items.First;
            // LinkedNode<LALRGItem> node = head;
            // 
            // 
            // bool docontinue;
            // do
            // {
            //     docontinue = false;
            //     LALRGItem item = node.Value;
            // 
            //     if (item.index != item.express.Length && !item.MarkSymbol.IsTerminal())
            //     {
            //         HashSet<int> lookahead;
            //         if (item.IsLastElement)
            //             lookahead = new HashSet<int>(item.lookahead);
            //         else
            //         {
            //             lookahead = grammar.GetFirst(item.express, item.index + 1);
            //             lookahead.Remove(ExpressInt.epsilon);
            //             lookahead.AddRange(item.lookahead);
            //         }
            // 
            //         foreach (GItem git in grammar.Value.FindAll(item.MarkSymbol))
            //         {
            //             LALRGItem newitem = new LALRGItem(git, lookahead);
            // 
            //             LinkedNode<LALRGItem> node2 = head;
            //         loop_2:
            //             if (newitem.Equals(node2.Value))
            //             {
            //                 HashSet<int> etlka = node2.Value.lookahead;
            //                 if (!etlka.IsSupersetOf(lookahead))
            //                 {
            //                     etlka.AddRange(lookahead);
            //                     docontinue = true;
            //                 }
            //                 continue;
            //             }
            // 
            //             node2 = node2.Next;
            //             if (node2 != head)
            //                 goto loop_2;
            // 
            //             items.Add(newitem);
            //             docontinue = true;
            //         }
            //     }
            // 
            //     if (docontinue)
            //         node = head;
            //     else
            //     {
            //         node = head.Next;
            //         if (node == head)
            //             break;
            //     }
            // } while (docontinue);



            // IEnumerator<LALRGItem> e1 = items.GetEnumerator();
            // IEnumerator<LALRGItem> e2 = items.GetEnumerator();
            // HashSet<LALRGItem> temp = new HashSet<LALRGItem>();
            // 
            // bool stop = false;
            // while (!stop && e1.MoveNext())
            // {
            //     LALRGItem item = e1.Current;
            //     if (item.Current.IsTerminal() == false) // non-terminal
            //     {
            //         HashSet<int> lookahead;
            //         if (item.IsLastElement)
            //             lookahead = new HashSet<int>(item.lookahead);
            //         else
            //         {
            //             lookahead = grammar.GetFirst(item.express, item.index + 1);
            //             lookahead.Remove(ExpressInt.epsilon);
            //             lookahead.AddRange(item.lookahead);
            //         }
            // 
            //         IEnumerator<GItem> e3 = grammar.Value.FindAll(item.Current).GetEnumerator();
            //         while (e3.MoveNext())
            //         {
            //             GItem temp2 = e3.Current;
            //             LALRGItem newitem = new LALRGItem(temp2, lookahead);
            //             while (e2.MoveNext())
            //                 if (newitem.Equals(e2.Current))
            //                 {
            //                     LALRGItem current = e2.Current;
            //                     if (current.lookahead.AddRange(lookahead))
            //                         goto continue_2;
            //                     goto continue_1;
            //                 }
            // 
            //             temp.Add(newitem);
            // 
            //         continue_2:
            //             stop = true;
            // 
            //         continue_1:
            //             e2.Reset();
            //         }
            //         e3.Dispose();
            //     }
            // }
            // 
            // e1.Dispose();
            // e2.Dispose();
            // 
            // items.AddRange(temp);
            // temp.Clear();
        }
    }
}
