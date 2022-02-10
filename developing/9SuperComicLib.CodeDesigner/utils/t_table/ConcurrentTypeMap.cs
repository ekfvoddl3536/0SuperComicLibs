using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperComicLib.CodeDesigner
{
#pragma warning restore
    internal sealed class ConcurrentTypeMap : IRuntimeTypeMapEX
    {
        private Semaphore semaphore = new Semaphore(1, 1);
        private Dictionary<HashedString, InternalTypeDesc> v = new Dictionary<HashedString, InternalTypeDesc>(16);

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
#endif

            HashedString hs = new HashedString(name);
            if (v.TryGetValue(hs, out InternalTypeDesc desc))
            {
                semaphore.WaitOne();
#if DEBUG
                if (!desc.TryAdd(value))
                    throw new ArgumentException("exist key");
#else
                desc.TryAdd(value);
#endif
            }
            else
            {
                semaphore.WaitOne();

                desc = new InternalTypeDesc();
                desc.TryAdd(value);
                v.Add(hs, desc);
            }

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

        public bool TryGet(string name, out ITypeDesc result)
        {
#if DEBUG
            if (name == null)
                throw new ArgumentNullException(nameof(name));
#endif
            if (v.TryGetValue(new HashedString(name), out InternalTypeDesc desc))
            {
                result = desc;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public ITypeDesc Get(string name)
        {
#if DEBUG
            if (name == null)
                throw new ArgumentNullException(nameof(name));
#endif
            return v[new HashedString(name)];
        }

        public IEnumerable<ITypeDesc> ToArray() => v.Values;

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
