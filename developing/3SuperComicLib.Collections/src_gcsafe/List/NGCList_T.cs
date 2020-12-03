using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SuperComicLib.Collections.GCSafe
{
    public class NGCList<T> : IList<T>, IDisposable
    {
        private Node head;
        private Node last;
        private T[] select;
        private int count;
        private int index;

        #region constructor
        public NGCList() : this(16) { }

        public NGCList(int initSize) =>
            head = new Node(select = new T[initSize.Max(16)]);

        public NGCList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            T[] vs = collection.ToArray();
            if ((count = index = vs.Length) < 16)
            {
                T[] nvs = new T[16];
                Array.Copy(vs, nvs, vs.Length);

                vs = nvs;
            }

            head = new Node(select = vs);
        }
        #endregion

        #region indexer
        public T this[int index]
        {
            get => Value(index);
            set => Value(index) = value;
        }

        public ref T Value(int index)
        {
#if DEBUG
            if ((uint)index >= (uint)count)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif
            Node node = head;

            int len = node.value.Length;
            while (index >= len)
            {
                index -= len;
                node = node.next;
            }

            return ref node.value[index];
        }
        #endregion

        #region property
        public int Capacity => GetCapacity();

        public int Count => count;

        public bool IsReadOnly => false;
        #endregion

        #region default methods
        public void Add(T item)
        {
            if (index == select.Length)
            {
                T[] vs = new T[index];
                Node newnode = new Node(vs);

                last.next = newnode;
                last = newnode;

                vs[0] = item;

                select = vs;
                index = 1;
            }
            else
                select[index++] = item;

            count++;
        }

        public void Clear()
        {
            Node node = head;

            int count = this.count;

            T[] vs = node.value;
            int len = vs.Length;
            while (count >= len)
            {
                for (int x = 0; x < len;)
                    vs[x++] = default;

                count -= len;

                node = node.next;
                vs = node.value;
            }

            while (--count >= 0)
                vs[count] = default;
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int count = array.Length - arrayIndex;
            if (count <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(array)} or {nameof(arrayIndex)}");

            Node node = head;

            T[] vs = node.value;
            int len = vs.Length;

            while (count >= len)
            {
                Array.Copy(vs, 0, array, arrayIndex, len);
                arrayIndex += len;
                count -= len;

                node = node.next;
                vs = node.value;
            }

            if (count > 0)
                Array.Copy(vs, 0, array, arrayIndex, count);
        }

        public int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;

            Node node = head;

            int count = this.count;

            T[] vs = node.value;

            int result = 0;
            int len = vs.Length, x;
            while (count >= len)
            {
                for (x = 0; x < len; result++)
                    if (comparer.Equals(vs[x++], item))
                        return result;

                count -= len;

                node = node.next;
                vs = node.value;
            }

            for (x = 0; x < count; result++)
                if (comparer.Equals(vs[x++], item))
                    return result;

            return -1;
        }

        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)count)
                throw new ArgumentOutOfRangeException(nameof(index));
            else if (index == count)
            {
                Add(item);
                return;
            }

            int len = select.Length;
            Node node;
            T[] sel;

            if (this.index == len)
            {
                sel = new T[len];

                node = new Node(sel);
                last.next = node;
                last = node;
                
                this.index = 0;
            }

            node = GetNode(ref index);
            sel = node.value;

            int fidx = len - 1;
            T temp = sel[fidx];

            Array.Copy(sel, index, sel, index + 1, len - index);
            sel[index] = item;

            while (node.next != null)
            {
                node = node.next;
                sel = node.value;
                T last = sel[fidx];

                Array.Copy(sel, 0, sel, 1, len);
                sel[0] = temp;
                temp = last;
            }

            count++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)count)
                throw new ArgumentOutOfRangeException(nameof(index));

            int len = select.Length;
            Node node = GetNode(ref index);

            T[] sel = node.value;
            sel[index] = default;

            Array.Copy(sel, index + 1, sel, index, len - index - 1);

            index = len - 1;
            while (node.next != null)
            {
                node = node.next;
                T[] next = node.value;
                sel[index] = next[0];

                Array.Copy(next, 1, next, 0, index);
                sel = next;
            }

            count--;
        }
        #endregion

        #region custom methods
        public void RemoveLast()
        {
            if (--count < 0)
                throw new InvalidOperationException();

            if (index == 0)
            {
                Node prev = GetLastPrevNode();
                T[] vs = prev.value;

                int len = vs.Length;
                vs[len - 1] = default;

                index = len;
                select = vs;
            }
            else
                select[--index] = default;
        }

        #endregion

        #region help methods
        /// <summary>
        /// optimize
        /// </summary>
        protected internal int GetCapacity()
        {
            int len = select.Length;
            int capacity = len;

            Node now = head;
            while (now.next != null)
            {
                now = now.next;
                capacity += len;
            }

            return capacity;
        }
        protected internal int GetNodeCount()
        {
            int nodes = 1;

            Node now = head;
            while (now.next != null)
            {
                now = now.next;
                nodes++;
            }

            return nodes;
        }

        protected internal Node GetLastPrevNode()
        {
            Node last2node = null;
            Node now = head;
            while (now.next != null)
            {
                last2node = now;
                now = now.next;
            }

            return last2node;
        }

        protected internal Node GetNode(int index) => GetNode(ref index);
        protected internal Node GetNode(ref int index)
        {
            int len = select.Length;

            Node result = head;
            for (; index >= len; index -= len)
                result = result.next;

            return result;
        }

        protected internal ref T GetNodeWithValue(int index, out Node result)
        {
            GetNodeWithValue(ref index, out result);
            return ref result.value[index];
        }
        protected internal ref T GetNodeWithValue(ref int index, out Node result)
        {
            int len = select.Length;

            result = head;
            for (; index >= len; index -= len)
                result = result.next;

            return ref result.value[index];
        }
        #endregion

        #region enumerable impl
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && head != null)
            {
                select = null;

                head.Dispose();
                last = head = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region struct
        private struct Enumerator : IEnumerator<T>
        {
            private NGCList<T> inst;
            private Node source;
            private T[] select;
            private int count;
            private int index;
            private readonly int version;
            private readonly int length;

            public Enumerator(NGCList<T> inst)
            {
                version = inst.count;

                Node source = inst.head;

                T[] select = source.value;
                length = select.Length;

                this.select = select;
                this.source = source;

                this.inst = inst;

                index = -1;
                count = 0;
            }

            public T Current => select[index];
            object IEnumerator.Current => select[index];

            public bool MoveNext()
            {
                if (inst.count != version)
                    throw new InvalidOperationException();
                else if (count++ < version)
                {
                    if (++index < length)
                        return true;
                    else if (source.next != null)
                    {
                        index = 0;
                        select = (source = source.next).value;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                source = inst.head;

                select = source.value;

                index = -1;
                count = 0;
            }

            public void Dispose()
            {
                source = null;
                select = null;
                inst = null;
            }
        }
        #endregion

        #region nested class
        protected internal class Node : IDisposable
        {
            public T[] value;
            public Node next;

            public Node(T[] value) => this.value = value;

            public void Dispose()
            {
                if (next != null)
                {
                    next.Dispose();
                    next = null;
                }
                value = null;
            }
        }
        #endregion
    }
}
