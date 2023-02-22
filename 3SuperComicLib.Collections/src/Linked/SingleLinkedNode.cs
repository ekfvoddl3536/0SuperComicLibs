namespace SuperComicLib.Collections
{
    public sealed class SingleLinkedNode<T>
    {
        public SingleLinkedNode<T> next;
        public T value;

        public SingleLinkedNode(T value) => this.value = value;
        public SingleLinkedNode(SingleLinkedNode<T> next, T value)
        {
            this.next = next;
            this.value = value;
        }
    }
}
