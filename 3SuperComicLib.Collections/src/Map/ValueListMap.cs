// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
