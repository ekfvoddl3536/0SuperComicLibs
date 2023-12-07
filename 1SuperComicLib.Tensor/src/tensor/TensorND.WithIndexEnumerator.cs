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
    public readonly struct WithIndexEnumerator : IEnumerator<ItemIndexTuple<T>>
    {
        private const long NO_ELEMENTS = -1, FIRST_ITEM = 0, NORMAL = 1;

        private readonly Enumerator _enumerator;
        private readonly uint[] _cDims;
        private readonly uint[] _bDims;
        private readonly long _flags;

        internal WithIndexEnumerator(TensorND<T> tensor)
        {
            _enumerator = new Enumerator(tensor);

            ref readonly var dims = ref tensor.dimsHnd;
            _bDims = dims.lpDims;

            var rank = dims._rank(0);
            _cDims = 
                rank == 0
                ? []
                : new uint[rank];

            _flags = dims.totalSize == 0 ? NO_ELEMENTS : FIRST_ITEM;
        }

        public ItemIndexTupleRef<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => new ItemIndexTupleRef<T>(ref _enumerator.Current, _cDims);
        }

        public bool MoveNext()
        {
            if (_cDims.Length == 0 || _flags < 0)
                return false;

            if (_flags == FIRST_ITEM)
            {
                Unsafe.AsRef(in _flags) = NORMAL;
                return _enumerator.MoveNext();
            }
            else if (_enumerator.MoveNext())
            {
                _next_dim();
                return true;
            }
            else
                return false;
        }

        private void _next_dim()
        {
            var cDims = _cDims;
            var bDims = _bDims;

            ref var cFirst = ref MemoryMarshal.GetArrayDataReference(cDims);
            ref var bFirst = ref MemoryMarshal.GetArrayDataReference(bDims);

            ref var c1 = ref Unsafe.Add(ref cFirst, cDims.Length - 1);
            ref var b1 = ref Unsafe.Add(ref bFirst, (bDims.Length >> 1) - 1);

            ref var c1e = ref Unsafe.Subtract(ref cFirst, 1);
            while (!Unsafe.AreSame(ref c1, ref c1e))
                if (c1 + 1 >= b1)
                {
                    // next
                    c1 = ref Unsafe.Subtract(ref c1, 1);
                    b1 = ref Unsafe.Subtract(ref b1, 1);
                }
                else
                {
                    ++c1;

                    var offset = Unsafe.ByteOffset(in cFirst, in c1) / sizeof(uint);
                    cDims.AsSpan((int)offset + 1).Clear();

                    break;
                }
        }

        ItemIndexTuple<T> IEnumerator<ItemIndexTuple<T>>.Current => new ItemIndexTuple<T>(ref _enumerator.Current, _cDims);
        object IEnumerator.Current => ((IEnumerator<ItemIndexTuple<T>>)this).Current;

        void IEnumerator.Reset() => throw new NotSupportedException(typeof(WithIndexEnumerator).FullName);
        void IDisposable.Dispose() { }
    }
}