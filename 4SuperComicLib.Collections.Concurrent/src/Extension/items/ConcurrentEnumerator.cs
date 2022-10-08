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

using SuperComicLib.Threading;
using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections.Concurrent
{
    public sealed class ConcurrentEnumerator<T>
    {
        private readonly IEnumerator<T> source;
        private SpinBlockLockSlim blockLock;

        public ConcurrentEnumerator(IEnumerator<T> enumerator)
        {
            System.Diagnostics.Contracts.Contract.Requires(enumerator != null);

            source = enumerator;
        }

        public bool MoveNext(out T item)
        {
            IEnumerator<T> src = source;

            ref SpinBlockLockSlim block = ref blockLock;

            block.Enter();
            try
            {
                bool result;
                if (result = src.MoveNext())
                    item = src.Current;
                else
                    item = default;

                block.Exit();

                return result;
            }
            catch (Exception exc)
            {
                block.Exit();
                throw exc;
            }
        }

        public void Reset()
        {
            ref SpinBlockLockSlim block = ref blockLock;

            block.Enter();
            try
            {
                source.Reset();
                block.Exit();
            }
            catch (Exception exc)
            {
                block.Exit();
                throw exc;
            }
        }

        public void Dispose()
        {
            source.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
