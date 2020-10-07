using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public static class NopTypeMap
    {
        public static readonly ITypeMap Default = new Instance();

        private sealed class Instance : ITypeMap
        {
            public bool IsSynchronized => true;
            public int Count => 0;

            public void Add<T>(string name) { }

            public void Add(string name, Type value) { }

            public bool Contains(string name) => false;

            public Type Get(string name) => null;

            public IEnumerable<Type> ToArray() => Array.Empty<Type>();

            public bool TryGet(string name, out Type result)
            {
                result = null;
                return false;
            }
        }
    }
}
