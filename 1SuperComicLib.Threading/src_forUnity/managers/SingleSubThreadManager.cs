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
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    internal sealed class SingleSubThreadManager : IDisposable
    {
        private Task task;

        public IEnumerator StartTask(Action<IWorker> asyncMethod)
        {
            SimpleThreadWorker e = new SimpleThreadWorker();
            task = Task.Factory.StartNew(SubThread, new Package(e, asyncMethod));
            return e;
        }

        private static void SubThread(object state)
        {
            Package p = (Package)state;

            p.method.Invoke(p.inst);
            p.inst.Dispose();
        }

        public void Dispose()
        {
            if (task != null)
            {
                Task t = task;
                if (!t.IsCompleted)
                    t.Wait();

                t.Dispose();
                task = null;
            }
            GC.SuppressFinalize(this);
        }

        #region block
        private sealed class Package
        {
            public readonly SimpleThreadWorker inst;
            public readonly Action<IWorker> method;

            public Package(SimpleThreadWorker inst, Action<IWorker> method)
            {
                this.inst = inst;
                this.method = method;
            }
        }
        #endregion
    }
}
