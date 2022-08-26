using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    internal readonly unsafe struct ReadOnlyRawContainerWrapper<T> : IReadOnlyRawContainer<T>
        where T : unmanaged
    {
        private readonly IRawContainer<T> container;

        public ReadOnlyRawContainerWrapper(IRawContainer<T> container) => this.container = container;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => container.begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => container.end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => container.rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => container.rend();

#if AnyCPU
        public ref readonly T this[size_t index] => ref container[index];
        public ref readonly T at(size_t index) => ref container.at(index);
        public size_t capacity() => container.capacity();
        public size_t size() => container.size();
#elif X86
        public ref readonly T this[int index] => ref container[index];
        public ref readonly T at(int index) => ref container.at(index);
        public int capacity() => container.capacity();
        public int size() => container.size();
#else
        public ref readonly T this[long index] => ref container[index];
        public ref readonly T at(long index) => ref container.at(index);
        public long capacity() => container.capacity();
        public long size() => container.size();
#endif
        public RawMemory getMemory() => container.getMemory();
    }
}
