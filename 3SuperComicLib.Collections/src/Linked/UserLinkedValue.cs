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

namespace SuperComicLib.Collections
{
    public sealed class UserLinkedValue<T>
    {
        public UserLinkedValue<T> Next;
        public UserLinkedValue<T> Prev;
        public T Value;

        public UserLinkedValue(UserLinkedValue<T> prev, UserLinkedValue<T> next, T value)
        {
            Prev = prev;
            Next = next;
            Value = value;
        }

        public UserLinkedValue(T value) => Value = value;

        public UserLinkedValue() { }

        public bool IsDisconnected => Next == null && Prev == null;

        public void SetHEAD() => Next = Prev = this;

        public void Disconnect()
        {
            ref UserLinkedValue<T> next = ref Next;
            ref UserLinkedValue<T> prev = ref Prev;
            if (next != null)
                next.Prev = prev;

            if (prev != null)
                prev.Next = next;

            next = null;
            prev = null;
            Value = default;
        }

        public void DisconnectAll()
        {
            if (IsDisconnected)
                throw new InvalidOperationException();

            UserLinkedValue<T> node = Next;
            for (; node != this && node != null; node = node.Next)
                node.Disconnect();

            Disconnect();
        }

        public void AddLast(UserLinkedValue<T> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (IsDisconnected)
                throw new InvalidOperationException();

            item.Next = this;
            item.Prev = Prev;
            Prev.Next = item;
            Prev = item;
        }

        public T[] ToArray()
        {
            if (IsDisconnected)
                throw new InvalidOperationException();

            int count = 1;
            UserLinkedValue<T> now = Next;
            for (; now != this; now = now.Next)
                count++;

            now = Prev;
            T[] result = new T[count];
            while (--count >= 0)
            {
                result[count] = now.Value;
                now = now.Prev;
            }

            return result;
        }

        public List<T> ToList()
        {
            if (IsDisconnected)
                throw new InvalidOperationException();

            List<T> result = new List<T> { Value };
            
            UserLinkedValue<T> now = Next;
            for (; now != this; now = now.Next)
                result.Add(now.Value);

            return result;
        }

        public IEnumerable<T> ToArrayFast() =>
            IsDisconnected 
            ? throw new InvalidOperationException() 
            : new Enumerator(this);

        private struct Enumerator : IEnumerable<T>, IEnumerator<T>
        {
            private UserLinkedValue<T> head;
            private UserLinkedValue<T> node;
            private T current;

            public Enumerator(UserLinkedValue<T> inst)
            {
                head = inst;
                node = inst;

                current = default;
            }

            public IEnumerator<T> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public T Current => current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                head = null;
                node = null;

                current = default;
            }

            public bool MoveNext()
            {
                if (node == null)
                    return false;

                current = node.Value;
                node = node.Next;
                if (node == head)
                    node = null;

                return true;
            }

            public void Reset()
            {
                current = default;
                node = head;
            }
        }
    }
}
