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

#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref struct _index_node<T>
        where T : unmanaged
    {
        // int next
        // int prev
        // T value
        internal readonly byte* _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal _index_node(byte* p) => _ptr = p;

#if AnyCPU
        public size_t next => *(size_t*)_ptr;
        public size_t prev => *(size_t*)(_ptr + sizeof(size_t));
        public ref T value => ref *(T*)(_ptr + sizeof(size_t) + sizeof(size_t));
#elif X86
        public int next => *(int*)_ptr;
        public int prev => *(int*)(_ptr + sizeof(int));
        public ref T value => ref *(T*)(_ptr + sizeof(long));
#else
        public long next => *(long*)_ptr;
        public long prev => *(long*)(_ptr + sizeof(long));
        public ref T value => ref *(T*)(_ptr + sizeof(long) + sizeof(long));
#endif

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(_index_node<T> left, _index_node<T> right) => left._ptr == right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(_index_node<T> left, _index_node<T> right) => left._ptr != right._ptr;
    }
}
