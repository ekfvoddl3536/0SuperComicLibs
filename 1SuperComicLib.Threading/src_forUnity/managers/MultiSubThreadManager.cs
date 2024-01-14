// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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
using System.Threading;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    internal sealed class MultiSubThreadManager : IDisposable
    {
        private Task[] tasks;
        private Action<int, IMultiWorker>[] methods;
        private volatile int nextMethodIndex;
        private readonly int sizeOfMethods;

        public MultiSubThreadManager(Action<int, IMultiWorker>[] taskMethods, int threadCount)
        {
            int msize = taskMethods.Length;

            methods = taskMethods;
            sizeOfMethods = msize;

            tasks = new Task[Math.Min(msize, threadCount)];
        }

        public IEnumerator StartTask()
        {
            Task[] vs = tasks;
            int len = vs.Length;

            ParallelWorker e = new ParallelWorker(len);
            for (int x = 0; x < len; x++)
                vs[x] = Task.Factory.StartNew(SubThread, new Package(e, x));

            return e;
        }

        private Action<int, IMultiWorker> TryGetNext()
        {
            int cachedIndex = Interlocked.Increment(ref nextMethodIndex);
            return
                cachedIndex >= sizeOfMethods
                ? null
                : methods[cachedIndex];
        }

        private void SubThread(object state)
        {
            Package p = (Package)state;

            int workerIndex = p.workerIndex;
            IMultiWorker inst = p.inst;

        loop:
            Action<int, IMultiWorker> current = TryGetNext();
            if (current != null)
            {
                current.Invoke(workerIndex, inst);
                goto loop;
            }

            if (p.inst.ExitThread())
                p.inst.Dispose();
        }

        public void Dispose()
        {
            if (tasks != null)
            {
                Task[] vs = tasks;
                int x = vs.Length;
                while (--x >= 0)
                {
                    Task t = vs[x];
                    if (!t.IsCompleted)
                        t.Wait();

                    t.Dispose();
                    vs[x] = null;
                }

                tasks = null;

                Action<int, IMultiWorker>[] acts = methods;
                x = acts.Length;
                while (--x >= 0)
                    acts[x] = null;

                methods = null;
            }
            GC.SuppressFinalize(this);
        }

        #region block
        private sealed class Package
        {
            public readonly ParallelWorker inst;
            public readonly int workerIndex;

            public Package(ParallelWorker inst, int workerIndex)
            {
                this.inst = inst;
                this.workerIndex = workerIndex;
            }
        }
        #endregion
    }
}
