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
    #region wrapper
    internal readonly struct IStackWrapper<T> : IStack<T>
    {
        private readonly Stack<T> inst;

        public IStackWrapper(Stack<T> inst) => this.inst = inst;

        #region un use
        public IEnumerator<T> GetEnumerator() => null;
        IEnumerator IEnumerable.GetEnumerator() => null;
        public int Capacity => 0;
        public T Peek() => default;
        public void Push(T item) { }
        public void Clear() { }
        public void Dispose() { }
        #endregion

        public int Count => inst.Count;

        public T Pop() => inst.Pop();
    }
    #endregion

    internal sealed class StackPopEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IStack<T> v;
        private T current;

        public StackPopEnumerable(Stack<T> s) =>
            v = new IStackWrapper<T>(s);

        public StackPopEnumerable(IStack<T> s) =>
            v = s;

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public T Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (v.Count > 0)
            {
                current = v.Pop();
                return true;
            }

            return false;
        }

        public void Reset() => throw new InvalidOperationException("can't reset");

        public void Dispose()
        {
            v = null;
            current = default;

            GC.SuppressFinalize(this);
        }
    }
}