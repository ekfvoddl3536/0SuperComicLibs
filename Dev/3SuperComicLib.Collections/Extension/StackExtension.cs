using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class StackExtension
    {
        public static void PushAll<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            IEnumerator<T> e1 = items.GetEnumerator();
            while (e1.MoveNext())
                stack.Push(e1.Current);
        }

        public static IEnumerable<T> PopEnumerator<T>(this Stack<T> stack) => new StackPopEnumerable<T>(stack);

        public static IEnumerable<T> PopEnumerator<T>(this IStack<T> stack) => new StackPopEnumerable<T>(stack);
    }
}
