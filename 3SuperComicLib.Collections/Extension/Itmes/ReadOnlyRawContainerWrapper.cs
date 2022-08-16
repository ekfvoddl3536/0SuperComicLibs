using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    internal readonly unsafe struct ReadOnlyRawContainerWrapper<T> : IReadOnlyRawContainer<T>
        where T : unmanaged
    {
        private readonly IRawContainer<T> container;

        public ReadOnlyRawContainerWrapper(IRawContainer<T> container) => this.container = container;

        public ref readonly T this[int index] => ref container[index];

        public ref readonly T at(int index) => ref container.at(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => container.begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => container.end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => container.rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => container.rend();

        public int capacity() => container.capacity();

        public int size() => container.size();

        public RawMemory getMemory() => container.getMemory();
    }
}
