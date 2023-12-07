// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

using System.Collections;

namespace SuperComicLib.Tensor;

public unsafe partial struct TensorND<T>
{
    [StructLayout(LayoutKind.Auto, Pack = 0x10)]
    public readonly struct Enumerator : IEnumerator<T>
    {
        private readonly long stride;
        private readonly long seq_size;
        private readonly long total;
        private readonly long seq_curr;

        private readonly nint ptr;
        private readonly nint bp;

        internal Enumerator(TensorND<T> tensor)
        {
            ref readonly var dims = ref tensor.dimsHnd;
            
            var stream = dims._stream_size();
            dims._rank_refs(stream.start, out stride);

            seq_curr = seq_size = stream.end;

            total = dims.totalSize;

            bp = ptr = (nint)(tensor.ptr - 1);
        }

        public ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => ref *(T*)ptr;
        }

        public bool MoveNext()
        {
            ref var total = ref Unsafe.AsRef(in this.total);
            if (total-- == 0)
                return false;

            ref var seq_curr = ref Unsafe.AsRef(in this.seq_curr);
            
            var bp = (T*)Unsafe.AsRef(in this.bp);

            var ptr = (T*)this.ptr;
            for (; ; )
                if (seq_curr-- == 0)
                {
                    seq_curr = seq_size;
                    ptr = bp += stride;
                }
                else
                {
                    ptr++;
                    break;
                }

            Unsafe.AsRef(in this.ptr) = (nint)ptr;
            Unsafe.AsRef(in this.bp) = (nint)bp;

            return true;
        }

        T IEnumerator<T>.Current => Current;
        object IEnumerator.Current => Current;

        void IEnumerator.Reset() => throw new NotSupportedException(typeof(Enumerator).FullName);
        void IDisposable.Dispose() { }
    }
}
