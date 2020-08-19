using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public interface IAddOnlyList<T>
    {
        void Add(T item);
    }

    public interface IIndexable
    {
        int Position { get; set;}
    }

    public interface IEnumerablePair<T1, T2> : IDisposable
    {
        T1 Item1 { get; }
        T2 Item2 { get; }
        bool MoveNext();
        void Reset();
    }

    public interface IForwardIterator<T> : IDisposable
    {
        void Add();
        void Reset();
        bool LazyAdd();
        bool IsAlive { get; }
        T Value { get; set; }
        T[] ToArray();
        int Count { get; }
    }

    public interface IArrayStream<T> : IIndexable, IDisposable
    {
        bool EndOfStream { get; }
        int Length { get; }
        T Read();
        IArrayStream<T> Read(int count);
        T Peek();
        IArrayStream<T> Peek(int count);
        void Reset();
        void Move();
        void Move(int count);
        bool CanRead(int count);
    }

    public interface IRangeRefArray<T> : IEnumerable<T>, IEnumerableSlim<T>
    {
        int Length { get; }
        T this[int index] { get; set; }
        T[] Source();
        IRangeRefArray<T> Slice(int begin, int count);
    }

    public interface IEnumerableSlim<T>
    {
        IForwardIterator<T> Begin();
        IForwardIterator<T> RBegin();
    }

    public interface ILongHashedList<T> : IDisposable, IEnumerable<T>, IEnumerableSlim<T>
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

    public interface IMap<T> : IDisposable, IEnumerable<KeyValuePair<int, T>>, IEnumerableSlim<T>
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

    public interface IIndexBag<T>
    {
        int Count { get; }
        int Capacity { get; }

        int this[T item] { get; }

        bool Contains(T item);
        int Find(T item);
        int AddOrGet(T item);
        bool Remove(T item);
        void Clear();
        T[] ToArray();
        bool TryAdd(T item, out int index);
    }

    public interface IUniObserver
    {
        bool IsNotUpdated { get; }

        bool IsUpdated { get; }

        bool IsAdded { get; }

        bool IsRemoved { get; }

        bool IsDisposed { get; }

        void Observe(object target);
    }
}
