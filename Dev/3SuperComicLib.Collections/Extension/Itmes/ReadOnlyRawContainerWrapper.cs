namespace SuperComicLib.Collections
{
    internal readonly unsafe struct ReadOnlyRawContainerWrapper<T> : IReadOnlyRawContainer<T>
        where T : unmanaged
    {
        private readonly IRawContainer<T> container;

        public ReadOnlyRawContainerWrapper(IRawContainer<T> container) => this.container = container;

        public ref readonly T this[int index] => ref container[index];

        public ref readonly T at(int index) => ref container.at(index);

        public RawConstIterator<T> cbegin() => new RawConstIterator<T>(container.begin().Value);
        public RawConstIterator<T> cend() => new RawConstIterator<T>(container.end().Value);

        public RawConstReverseIterator<T> crbegin() => new RawConstReverseIterator<T>(container.rbegin().Value);
        public RawConstReverseIterator<T> crend() => new RawConstReverseIterator<T>(container.rend().Value);

        public int capacity() => container.capacity();

        public int size() => container.size();

        public RawMemory getMemory() => container.getMemory();
    }
}
