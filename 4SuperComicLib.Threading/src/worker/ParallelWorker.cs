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
using System.Collections.Concurrent;
using System.Threading;

namespace SuperComicLib.Threading
{
    internal sealed unsafe class ParallelWorker : WorkerBase, IMultiWorker
    {
        private AutoResetEvent[] ares;
        private Action[] actions;
        private ConcurrentStack<int> indexes;
        private volatile int count;
        private volatile int runningThreads;

        public ParallelWorker(int size)
        {
            indexes = new ConcurrentStack<int>();
            actions = new Action[size];

            runningThreads = size;

            AutoResetEvent[] vs = new AutoResetEvent[size];
            while (--size >= 0)
                vs[size] = new AutoResetEvent(false);

            ares = vs;

        }

        public bool ExitThread() => Interlocked.Decrement(ref runningThreads) <= 0;

        public Awaiter Invoke(int handle, Action action)
        {
            actions[handle] = action;
            skipOne = false;

            indexes.Push(handle);
            Interlocked.Increment(ref count);

            return new Awaiter(ares[handle]);
        }

        protected override void OnProcess()
        {
            int cached_count = count;

            AutoResetEvent[] ares = this.ares;
            Action[] actions = this.actions;
            ConcurrentStack<int> indexes = this.indexes;

            int x = cached_count;
            while (--x >= 0 && indexes.TryPop(out int idx))
            {
                actions[idx].Invoke();
                ares[idx].Set();
            }

            Interlocked.Add(ref count, -cached_count);
        }

        protected override void Dispose(bool disposing)
        {
#if DEBUG
            if (runningThreads > 0)
                throw new InvalidOperationException();
#endif
            if (ares != null)
            {
                int length = ares.Length;
                int k = 0;
                for (; k < length; k++)
                {
                    ares[k].Dispose();
                    ares[k] = null;
                }

                ares = null;

                for (k = 0; k < length; k++)
                    actions[k] = null;

                actions = null;

                indexes.Clear();
                indexes = null;

                base.Dispose(disposing);
            }
        }
    }
}
