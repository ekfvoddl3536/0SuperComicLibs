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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    public static class ParallelEx
    {
        public static int EffectiveMaxConcurrencyLevel(TaskScheduler taskScheduler, int value)
        {
            uint max = (uint)taskScheduler.MaximumConcurrencyLevel;
            return
                max < int.MaxValue
                ? (int)CMath.Min(max, (uint)value)
                : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EffectiveMaxConcurrencyLevel(int value) => EffectiveMaxConcurrencyLevel(TaskScheduler.Current, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void While(Func<bool> body) =>
            While(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, body);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void While(int worker_count, Func<bool> body) =>
            While(new ParallelOptions { MaxDegreeOfParallelism = worker_count }, body);

        public static void While(ParallelOptions option, Func<bool> body)
        {
            Action[] acts = new Action[option.MaxDegreeOfParallelism];
            Action local = new SetWhile(body).CB;
            for (int i = option.MaxDegreeOfParallelism; --i >= 0;)
                acts[i] = local;

            Parallel.Invoke(option, acts);
        }

        public static void Invoke(InvokeCallback body) =>
            Invoke(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, body);

        public static void Invoke(int worker_count, InvokeCallback body) =>
            Invoke(new ParallelOptions { MaxDegreeOfParallelism = worker_count }, body);

        public static void Invoke(ParallelOptions option, InvokeCallback body)
        {
            int count = option.MaxDegreeOfParallelism;
            Action[] acts = new Action[count];
            for (int i = count; --i >= 0;)
                acts[i] = new InvokeRunner(body, i, count).Run;

            Parallel.Invoke(option, acts);
        }

        public static void BatchFor(
            int from_inclusive,
            int to_exclusive,
            int batch_size,
            ParallelOptions options,
            Action<int, BatchLoopState> body)
        {
            int add_idx = batch_size * (options.MaxDegreeOfParallelism - 1);

            Action[] acts = new Action[options.MaxDegreeOfParallelism];

            BatchLoopState state = new BatchLoopState();

            for (int i = options.MaxDegreeOfParallelism; --i >= 0;)
                acts[i] = new BatchForWorker(from_inclusive * batch_size, to_exclusive, add_idx, batch_size, body, state).Run;

            Parallel.Invoke(options, acts);
        }

        private sealed class InvokeRunner
        {
            private readonly InvokeCallback m_body;
            private readonly int m_thread_index;
            private readonly int m_thread_count;

            public InvokeRunner(InvokeCallback body, int index, int count)
            {
                m_body = body;
                m_thread_index = index;
                m_thread_count = count;
            }

            public void Run() => m_body.Invoke(m_thread_index, m_thread_count);
        }

        private struct SetWhile
        {
            private readonly Func<bool> body;
            private volatile int block;

            public SetWhile(Func<bool> body)
            {
                this.body = body;
                block = default;
            }

            public void CB()
            {
                while (block == 0 && body.Invoke())
                    ;

                Interlocked.Exchange(ref block, 1);
            }
        }

        private sealed class BatchForWorker
        {
            private readonly static Func<int, int, bool> pred_reqBreak = new Func<int, int, bool>(Min);

            public int sidx_cur;
            public int sidx_add;
            public int eidx;
            public int batch_sz;
            public Action<int, BatchLoopState> body;
            public BatchLoopState state;


            public BatchForWorker(int sidx, int eidx, int add, int bsize, Action<int, BatchLoopState> body, BatchLoopState state)
            {
                sidx_cur = sidx;
                sidx_add = add;

                batch_sz = bsize;

                this.eidx = eidx;

                this.body = body;
                this.state = state;
            }

            public void Run()
            {
                int sidx = sidx_cur;
                int add = sidx_add;
                var body = this.body;
                var state = this.state;

                var pred = pred_reqBreak;

                int bsize = batch_sz;
                for (int i = eidx; sidx < i && state.NeedBreak(sidx); sidx += add)
                    for (int j = 0; j++ < bsize && state.NeedBreak(sidx); sidx++)
                    {
                        body.Invoke(sidx, state);

                        if (state.m_state == BatchLoopState.STATE_BREAK)
                            state.m_lowestBreak.ExchangeIf(sidx, pred);
                    }
            }

            private static bool Min(int old, int @new) => old > @new;
        }
    }
}