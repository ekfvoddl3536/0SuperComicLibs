using System.Diagnostics;

namespace SuperComicLib.Collections
{
    internal sealed class IIterableView<T>
    {
        private readonly T[] arr;

        public IIterableView(IIterable<T> inst) => arr = inst.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => arr;
    }
}