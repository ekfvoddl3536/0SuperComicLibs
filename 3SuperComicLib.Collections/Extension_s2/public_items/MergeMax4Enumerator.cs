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
    public sealed unsafe class MergeMax4Enumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> first;
        private readonly IEnumerator<T> second;
        private IEnumerator<T> opt_e1;
        private IEnumerator<T> opt_e2;
        private IEnumerator<T> current;
        private byte* state;

        public MergeMax4Enumerator(IEnumerator<T> first, IEnumerator<T> second)
        {
            this.first = first;
            this.second = second;
            opt_e1 = null;
            opt_e2 = null;
            current = first;
            state = null;
        }

        public IEnumerator<T> MergeCoreN(IEnumerator<T> other)
        {
            if (opt_e1 == null)
                opt_e1 = other;
            else if (opt_e2 == null)
                opt_e2 = other;
            else
                return new MergeMax4Enumerator<T>(this, other);

            return this;
        }

        public IEnumerator<T> AppendCore1(T element)
        {
            var last = opt_e2 ?? opt_e1 ?? second;
            if (last is ScalableEnumerator<T> ade)
            {
                ade.AppendCore1(element);
                return this;
            }

            return MergeCoreN(new ScalableEnumerator<T>(element));
        }

        public IEnumerator<T> PrependCore1(T element)
        {
            if (first is ScalableEnumerator<T> ade)
            {
                ade.PrependCore1(element);
                return this;
            }

            return new MergeMax4Enumerator<T>(new ScalableEnumerator<T>(element), this);
        }

        public IEnumerator<T> LastEnumerator() => opt_e2 ?? opt_e1 ?? second;

        T IEnumerator<T>.Current => current.Current;
        object IEnumerator.Current => current.Current;

        bool IEnumerator.MoveNext()
        {
            for (; !current.MoveNext(); ++state)
            {
                switch ((int)state)
                {
                    case 0:
                        current = second;
                        break;

                    case 1:
                        current = opt_e1;
                        break;

                    case 2:
                        current = opt_e2;
                        break;

                    default:
                        return false;
                }

                if (current == null)
                    return false;
            }

            return true;
        }

        void IEnumerator.Reset()
        {
            first.Reset();
            second.Reset();

            opt_e1?.Reset();
            opt_e2?.Reset();

            current = first;
            state = null;
        }

        void IDisposable.Dispose()
        {
            first.Dispose();
            second.Dispose();

            opt_e1?.Dispose();
            opt_e2?.Dispose();

            current = null;
        }
    }
}
