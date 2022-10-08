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
using System.Threading;

namespace SuperComicLib.Collections.Concurrent
{
    public sealed class ParallelEnumeratorDef<T> : ParallelEnumerate<T>
    {
        private Action<T, CancellationTokenSource> callback;
        private CancellationTokenSource cancellation;

        public ParallelEnumeratorDef(Action<T, CancellationTokenSource> callback)
        {
            this.callback = callback;

            cancellation = new CancellationTokenSource();
        }

        protected override bool OnEnumerate(T current, int thread_idx)
        {
            CancellationTokenSource cancellation = this.cancellation;

            callback.Invoke(current, cancellation);
            return !cancellation.IsCancellationRequested;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && cancellation != null)
            {
                callback = null;

                cancellation.Dispose();
                cancellation = null;
            }
        }
    }
}
