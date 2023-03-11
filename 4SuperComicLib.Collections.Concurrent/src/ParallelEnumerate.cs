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
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperComicLib.Threading;
using SuperComicLib.CodeContracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections.Concurrent
{
    public abstract class ParallelEnumerate<T> : IDisposable
    {
        public void Open(IEnumerable<T> files) =>
            Open(files, Environment.ProcessorCount);

        public void Open(IEnumerable<T> files, int threadCount)
        {
            FastContract.Requires(files != null);
            FastContract.Requires(threadCount > 0 && threadCount <= Environment.ProcessorCount);

            var origin = files.GetEnumerator() ?? throw new ArgumentNullException($"{nameof(files)}.{nameof(files.GetEnumerator)}() --> null");
            OnBeginEnumerate(threadCount);

            var vs = new ConcurrentEnumerator<T>(origin);
            var token = new StrongBox<VolatileInt32>(default);

            var methods = new Action[threadCount];
            for (int x = 0; x < threadCount; x++)
                methods[x] = new Worker(this, vs, token, x).Run;

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = threadCount }, methods);

            vs.Dispose();

            OnEndEnumerate();
        }

        /// <summary>
        /// Parallel item enumerate
        /// </summary>
        /// <returns>return false => break loop</returns>
        protected abstract bool OnEnumerate(T current, int thread_idx);

        protected virtual void OnBeginEnumerate(int threadCount)
        {
        }

        protected virtual void OnExitThread(int thread_idx)
        {
        }

        protected virtual void OnEndEnumerate()
        {
        }

        #region dispose
        ~ParallelEnumerate()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

        #region inner class
        private sealed class Worker
        {
            public readonly ParallelEnumerate<T> owner;
            public readonly ConcurrentEnumerator<T> vs;
            public readonly StrongBox<VolatileInt32> tokenSrc;
            public readonly int thread_idx;

            public Worker(
                ParallelEnumerate<T> owner,
                ConcurrentEnumerator<T> vs,
                StrongBox<VolatileInt32> tokenSrc,
                int thread_idx)
            {
                this.owner = owner;
                this.vs = vs;
                this.tokenSrc = tokenSrc;

                this.thread_idx = thread_idx;
            }

            public void Run()
            {
                var _inst = owner;
                var _iter = vs;
                var _idx = thread_idx;
                ref var _tok = ref tokenSrc.Value;

                while (_iter.MoveNext(out T current_) && _tok == 0)
                    if (!_inst.OnEnumerate(current_, _idx))
                    {
                        _tok.Value = 1;
                        break;
                    }

                _inst.OnExitThread(_idx);
            }
        }
        #endregion
    }
}
