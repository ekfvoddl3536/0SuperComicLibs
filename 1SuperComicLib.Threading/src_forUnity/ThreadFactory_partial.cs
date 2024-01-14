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

namespace SuperComicLib.Threading
{
    partial class ThreadFactory
    {
        #region classes
        #region base
        private abstract class E_Base : IEnumerator, IDisposable
        {
            private IEnumerator e;

            protected void Init(IEnumerator e) => this.e = e;

            public object Current => null;

            public bool MoveNext()
            {
                if (e.MoveNext())
                    return true;

                Dispose();
                return false;
            }

            public virtual void Reset() { }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (e is IDisposable ed)
                    ed.Dispose();

                e = null;
            }
        }
        #endregion

        #region e0
        private sealed class E_Single : E_Base
        {
            private SingleSubThreadManager inst;

            public E_Single(Action<IWorker> taskMethod)
            {
                SingleSubThreadManager v = new SingleSubThreadManager();
                inst = v;
                Init(v.StartTask(taskMethod));
            }

            protected override void Dispose(bool disposing)
            {
                if (inst != null)
                {
                    inst.Dispose();
                    inst = null;

                    base.Dispose(disposing);
                }
            }
        }
        #endregion

        #region e1
        private sealed class E_Multi : E_Base
        {
            private MultiSubThreadManager inst;

            public E_Multi(Action<int, IMultiWorker>[] taskMethods, int threadSize)
            {
                MultiSubThreadManager v = new MultiSubThreadManager(taskMethods, threadSize);
                inst = v;
                Init(v.StartTask());
            }

            protected override void Dispose(bool disposing)
            {
                if (inst != null)
                {
                    inst.Dispose();
                    inst = null;

                    base.Dispose(disposing);
                }
            }
        }
        #endregion
        #endregion
    }
}
