// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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
    public sealed class ScalableEnumerator<T> : IEnumerator<T>
    {
        private SingleLinkedNode<T> first;
        private SingleLinkedNode<T> last;
        private SingleLinkedNode<T> pcur;

        public ScalableEnumerator(T element)
        {
            first = new SingleLinkedNode<T>(element);
            last = first;
            pcur = first;
        }

        public IEnumerator<T> AppendCore1(T element)
        {
            var item = new SingleLinkedNode<T>(element);

            last.next = item;
            last = item;

            return this;
        }

        public IEnumerator<T> PrependCore1(T element)
        {
            var item = new SingleLinkedNode<T>(first, element);

            if (pcur == first)
                pcur = item;

            first = item;

            return this;
        }

        T IEnumerator<T>.Current => pcur.value;
        object IEnumerator.Current => pcur.value;

        bool IEnumerator.MoveNext()
        {
            var after = pcur;
            if (after != null)
            {
                pcur = after.next;
                return true;
            }

            return false;
        }

        void IEnumerator.Reset() => pcur = first;

        void IDisposable.Dispose()
        {
            // cut reference
            for (var p = first; p != null;)
            {
                var t = p.next;
                p.next = null;

                GC.SuppressFinalize(p);

                p = t;
            }

            GC.SuppressFinalize(this);
        }
    }
}
