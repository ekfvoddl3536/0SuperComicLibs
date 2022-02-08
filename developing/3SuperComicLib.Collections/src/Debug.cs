using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SuperComicLib.Collections
{
    internal sealed class IIterableView<T>
    {
        private readonly T[] arr;

        public IIterableView(IValueIterable<T> inst) => arr = inst.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => arr;
    }

    internal sealed class EnumerableView<T>
    {
        private readonly T[] arr;

        public EnumerableView(IEnumerable<T> inst) => arr = inst.ToArray();


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => arr;
    }
}