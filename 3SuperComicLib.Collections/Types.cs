﻿#pragma warning disable IDE1006 // 명명 스타일
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

    public interface ILongHashedList<T> : IDisposable, IEnumerable<T>
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

    public interface IMap<T> : IDisposable, IEnumerable<KeyValuePair<int, T>>
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
        int capacity();

        int size();

        RawMemory getMemory();
    }

    public interface IReadOnlyRawContainer<T> : IRawContainer
    where T : unmanaged
    {
        ref readonly T this[int index] { get; }

        ref readonly T at(int index);

        const_iterator<T> cbegin();
        const_iterator<T> cend();

        const_reverse_iterator<T> crbegin();
        const_reverse_iterator<T> crend();
    }

    public interface IRawContainer<T> : IRawContainer
        where T : unmanaged
    {
        ref T this[int index] { get; }

        ref T at(int index);

        _iterator<T> begin();
        _iterator<T> end();

        reverse_iterator<T> rbegin();
        reverse_iterator<T> rend();
    }

    public interface IRawList<T> : IRawContainer<T>
        where T : unmanaged
    {
        void push_back(in T item);

        T pop_back();

        void insert(int index, in T item);

        bool removeAt(int index);

        void earse(_iterator<T> position);

        void earse(_iterator<T> first, _iterator<T> last);
    }

    public interface IRawAllocater
    {
        bool IsPersistent { get; }

        IntPtr stdAlloc(int cb, bool initDefault);

        void stdFree(IntPtr ptr);
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
