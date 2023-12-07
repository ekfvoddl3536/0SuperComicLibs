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

namespace SuperComicLib;

public readonly unsafe struct PinnedArray<T> : IEquatable<PinnedArray<T>>, IEnumerable<T>
    where T : unmanaged
{
    public readonly T* ptr;
    public readonly T[] memory;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PinnedArray(int length)
    {
        if (length == 0)
        {
            ptr = null;
            memory = [];
        }
        else
        {
            memory = GC.AllocateArray<T>(length, true);
            ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(memory));
        }
    }

    public int Length => memory.Length;

    public long LongLength => memory.LongLength;

    public ref T this[long index] => ref ptr[index];

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)memory).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => memory.GetEnumerator();

    public Span<T> AsSpan() => new Span<T>(ptr, memory.Length);

    public bool Equals(PinnedArray<T> other) => this == other;

    public override bool Equals(object? obj) => 
        obj is PinnedArray<T> other && Equals(other);
    public override int GetHashCode() =>
        ((nint)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(memory))).GetHashCode();

    public static bool operator ==(PinnedArray<T> left, PinnedArray<T> right) => left.memory == right.memory;
    public static bool operator !=(PinnedArray<T> left, PinnedArray<T> right) => left.memory != right.memory;
}
