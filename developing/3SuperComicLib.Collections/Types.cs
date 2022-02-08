using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public interface IAddOnlyList<T>
    {
        void Add(T item);
    }

    public interface IHashCodeUpdater<T>
    {
        T Item { get; }
        int GetOldHashCode();
        int GetNewHashCode();
        void Apply();
    }

    public interface IEnumerablePair<T1, T2>
    {
        IEnumeratorPair<T1, T2> GetEnumeratorPair();
    }

    public interface IEnumeratorPair<T1, T2> : IDisposable
    {
        T1 Item1 { get; }
        T2 Item2 { get; }
        bool MoveNext();
        void Reset();
    }

    public interface IValueIterator<T> : IDisposable
    {
        void Add();
        void Reset();
        bool LazyAdd();
        bool IsAlive { get; }
        T Value { get; set; }
        T[] ToArray();
        int Count { get; }
    }

    public interface IArrayStream<T> : IDisposable
    {
        bool EndOfStream { get; }
        int Length { get; }
        int Position { get; set; }
        T Read();
        IArrayStream<T> Read(int count);
        T Peek();
        IArrayStream<T> Peek(int count);
        void Reset();
        void Move();
        void Move(int count);
        bool CanRead(int count);
    }

    public interface IRangeRefArray<T> : IEnumerable<T>, IValueIterable<T>
    {
        int Length { get; }
        T this[int index] { get; set; }
        T[] Source();
        IRangeRefArray<T> Slice(int begin, int count);
    }

    public interface IValueIterable<T>
    {
        IValueIterator<T> Begin();
        IValueIterator<T> RBegin();
        T[] ToArray();
    }

    public interface ILongHashedList<T> : IDisposable, IEnumerable<T>, IValueIterable<T>
    {
        int Count { get; }
        int Capacity { get; }

        IEnumerable<long> Keys { get; }
        IEnumerable<T> Values { get; }

        T this[long hashcode] { get; set; }
        bool TryGet(long hashcode, out T result);
        bool TrySet(long hashcode, T newvalue);
        bool Contains(long hashcode);
        void Add(long hashcode, T value);
        void Remove(long hashcode);
        void Clear();
    }

    public interface IMap<T> : IDisposable, IEnumerable<KeyValuePair<int, T>>, IValueIterable<T>
    {
        int Count { get; }
        int Capacity { get; }

        bool TryGet(int hashcode, out T result);
        bool Contains(int hashcode);
        void Add(int hashcode, T value);
        T Get(int hashcode);
        void Remove(int hashcode);
        void Clear();

        IEnumerable<int> Keys { get; }
        IEnumerable<T> Values { get; }
    }

    public interface IStack<T> : IDisposable, IEnumerable<T>
    {
        int Count { get; }
        int Capacity { get; }

        void Clear();

        void Push(T item);

        T Pop();

        T Peek();
    }

    public interface IUniObserver
    {
        bool IsNotUpdated { get; }

        bool IsUpdated { get; }

        bool IsAdded { get; }

        bool IsRemoved { get; }

        bool IsDisposed { get; }

        void Observe(object target);

        void Update();
    }

    public interface ICountObserver : IUniObserver
    {
        int Count { get; }
    }

    public interface IRawContainer
    {
#pragma warning disable IDE1006
        int capacity();

        int size();

        RawMemory getMemory();
#pragma warning restore IDE1006 // 명명규칙
    }

    public interface IRawContainer<T> : IRawContainer
        where T : unmanaged
    {
        ref T this[int index] { get; }

        ref T At(int index);

#pragma warning disable IDE1006
        RawIterator<T> begin();
        RawIterator<T> end();

        RawReverseIterator<T> rbegin();
        RawReverseIterator<T> rend();
#pragma warning restore IDE1006 // 명명규칙
    }

    public interface IRawList<T> : IRawContainer<T>
        where T : unmanaged
    {
#pragma warning disable IDE1006
        void push_back(T item);

        T pop_back();

        void insert(int index, T item);

        bool removeAt(int index);

        void earse(RawIterator<T> position);

        void earse(RawIterator<T> first, RawIterator<T> last);
#pragma warning restore IDE1006 // 명명규칙
    }

    public interface IRawAllocater
    {
#pragma warning disable IDE1006
        IntPtr stdAlloc(int cb, bool initDefault);

        void stdFree(IntPtr ptr);
#pragma warning restore IDE1006 // 명명규칙
    }

    public interface IRawPersistentAllocater : IRawAllocater
    {
    }

    internal interface IByReferenceIndexer_Internal<T>
    {
        ref T ByRefValue(int index);
    }

    internal interface ILinkedListSlim_Internal<T> : IByReferenceIndexer_Internal<T>
    {
        int GetNextNode(int node);

        int GetPrevNode(int node);
    }
}
