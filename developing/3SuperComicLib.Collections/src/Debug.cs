#if DEBUG
using System.Collections.Generic;
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    internal sealed class LinkedHashSetView<T>
    {
        private readonly LinkedHashSet<T> inst;

        public LinkedHashSetView(LinkedHashSet<T> inst) => this.inst = inst;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => inst.ToArray();
    }

    internal sealed class DEBUG_StackView<T>
    {
        private readonly T[] inst;

        public DEBUG_StackView(FixedStack<T> inst) => this.inst = inst.ToArray();

        public DEBUG_StackView(LookaheadStack<T> inst) => this.inst = inst.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => inst;
    }

    internal sealed class DEBUG_QueueView<T>
    {
        private readonly T[] inst;

        public DEBUG_QueueView(FixedQueue<T> inst) => this.inst = inst.ToArray();

        public DEBUG_QueueView(LookaheadQueue<T> inst) => this.inst = inst.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => inst;
    }

    internal sealed class MapView<T>
    {
        private readonly Map<T> inst;

        public MapView(Map<T> inst) => this.inst = inst;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<int, T>[] Items => inst.ToArray();
    }

    internal sealed class LongHashedListView<T>
    {
        private readonly LongHashedList<T> inst;

        public LongHashedListView(LongHashedList<T> inst) => this.inst = inst;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => inst.ToArray();
    }

    internal sealed class CHashSetView<T>
    {
        private readonly CHashSet<T> inst;

        public CHashSetView(CHashSet<T> inst) => this.inst = inst;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => inst.ToArray();
    }
}
#endif
