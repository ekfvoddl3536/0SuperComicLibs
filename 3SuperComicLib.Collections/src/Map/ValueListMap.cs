using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class ValueListMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        private readonly Dictionary<TKey, List<TValue>> _srcMap;

        public ValueListMap() =>
            _srcMap = new Dictionary<TKey, List<TValue>>();

        public ValueListMap(int capacity) =>
            _srcMap = new Dictionary<TKey, List<TValue>>(capacity);

        public ValueListMap(IEqualityComparer<TKey> comparer) =>
            _srcMap = new Dictionary<TKey, List<TValue>>(comparer);

        public ValueListMap(int capacity, IEqualityComparer<TKey> comparer) =>
            _srcMap = new Dictionary<TKey, List<TValue>>(capacity, comparer);

        public List<TValue> this[TKey key] => _srcMap[key];

        public List<TValue> Add(TKey key, TValue value)
        {
            var m = _srcMap;
            if (m.TryGetValue(key, out List<TValue> list))
                list.Add(value);
            else
            {
                list = FreeListPool<TValue>.GetOrCreate();
                list.Add(value);
                m.Add(key, list);
            }

            return list;
        }

        public bool ContainsKey(TKey key) =>
            _srcMap.ContainsKey(key);

        public void Clear()
        {
            var e = _srcMap.Values.GetEnumerator();
            while (e.MoveNext())
                FreeListPool<TValue>.Return(e.Current);

            e.Dispose();

            _srcMap.Clear();
        }

        public IEnumerable<TKey> Keys => _srcMap.Keys;

        public IEnumerable<List<TValue>> Values => _srcMap.Values;

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() => _srcMap.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        ~ValueListMap()
        {
            Clear();
        }
    }
}
