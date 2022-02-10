using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class FreeListPool<T>
    {
        private static readonly Queue<List<T>> freeLists = new Queue<List<T>>();

        public static List<T> GetOrCreate() =>
            freeLists.Count > 0 
            ? freeLists.Dequeue() 
            : new List<T>();

        public static void Return(List<T> list)
        {
            list.Clear();
            freeLists.Enqueue(list);
        }
    }
}
