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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SuperComicLib.Collections
{
    public class ConcurrentQueueSlim<T> : IEnumerable<T>
    {
        private Node m_head;
        private Node m_tail;
        private int m_count;
        private int m_version;
        private volatile int m_barrier;

        public int Count => m_count;

        public int Version => m_version;

        public void Enqueue(T value)
        {
            Node new_node = new Node(value);

            for (SpinWait w = default; m_barrier != 0 || Interlocked.CompareExchange(ref m_barrier, 1, 0) != 0;)
                w.SpinOnce();

            if (m_tail == null)
                m_head = new_node;
            else
                m_tail.next = new_node;

            m_tail = new_node;

            m_count++;
            m_version++;

            Interlocked.Exchange(ref m_barrier, 0);
        }

        public bool TryDequeue(out T result)
        {
            if (m_count <= 0)
            {
                result = default;
                return false;
            }

            for (SpinWait w = default; m_barrier != 0 || Interlocked.CompareExchange(ref m_barrier, 1, 0) != 0;)
                w.SpinOnce();

            result = m_head.value;

            if ((m_head = m_head.next) == null)
                m_tail = null;

            m_count--;
            m_version++;

            Interlocked.Exchange(ref m_barrier, 0);

            return true;
        }

        public void Clear()
        {
            if (m_count <= 0)
                return;

            for (SpinWait w = default; m_barrier != 0 || Interlocked.CompareExchange(ref m_barrier, 1, 0) != 0;)
                w.SpinOnce();

            for (Node now = m_head; now.next != null;)
            {
                Node c = now.next;
                now.next = null;
                now = c;
            }

            m_head = null;
            m_tail = null;

            m_count = 0;
            m_version++;

            Interlocked.Exchange(ref m_barrier, 0);
        }

        public T Dequeue() => 
            TryDequeue(out T result) 
            ? result 
            : throw new InvalidOperationException("empty list");

        private sealed class Node
        {
            public Node next;
            public T value;

            public Node(T value) => this.value = value;
        }

        public IEnumerator<T> GetEnumerator() => new Internal_Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected sealed class Internal_Enumerator : IEnumerator<T>
        {
            private ConcurrentQueueSlim<T> inst;
            private Node now;
            private T current;
            private readonly int version;

            public Internal_Enumerator(ConcurrentQueueSlim<T> inst)
            {
                this.inst = inst;
                now = inst.m_head;
                version = inst.m_version;
                current = default;
            }

            public T Current => current;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (inst.m_version != version)
                    throw new InvalidOperationException("list modified");

                if (now != null)
                {
                    current = now.value;
                    now = now.next;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                if (inst.m_version != version)
                    throw new InvalidOperationException("list modified");

                now = inst.m_head;
            }

            public void Dispose()
            {
                inst = null;
                now = null;
            }
        }
    }
}
