using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperComicLib.CodeDesigner
{
#pragma warning restore
    internal sealed class ConcurrentTypeMap : IRuntimeTypeMapEX
    {
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private Dictionary<HashedString, Type> v = new Dictionary<HashedString, Type>(32);

        public bool IsSynchronized => true;
        public int Count => v.Count;

        public void Add<T>(string name) => Add(name, typeof(T));

        public void Add(string name, Type value)
        {
#if DEBUG
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            HashedString hs = new HashedString(name);
            if (v.ContainsKey(hs))
                throw new ArgumentException("Exist Key");

            semaphore.Wait();
            v.Add(hs, value);
#else
                semaphore.Wait();
                v.Add(new HashedString(name), value);
#endif
            semaphore.Release();
        }

        public bool Contains(string name)
        {
#if DEBUG
            if (name == null)
                throw new ArgumentNullException(nameof(name));
#endif
            return v.ContainsKey(new HashedString(name));
        }

        public bool TryGet(string name, out Type result)
        {
#if DEBUG
            if (name == null)
                throw new ArgumentNullException(nameof(name));
#endif
            return v.TryGetValue(new HashedString(name), out result);
        }

        public Type Get(string name)
        {
#if DEBUG
            if (name == null)
                throw new ArgumentNullException(nameof(name));
#endif
            return v[new HashedString(name)];
        }

        public IEnumerable<Type> ToArray() => v.Values;

        public void Dispose()
        {
            if (semaphore != null)
            {
                semaphore.Dispose();
                semaphore = null;

                v.Clear();
                v = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
