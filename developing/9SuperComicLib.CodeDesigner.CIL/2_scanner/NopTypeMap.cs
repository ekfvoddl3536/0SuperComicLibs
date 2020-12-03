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

            public ITypeDesc Get(string name) => null;

            public IEnumerable<ITypeDesc> ToArray() => Array.Empty<ITypeDesc>();

            public bool TryGet(string name, out ITypeDesc result)
            {
                result = null;
                return false;
            }
        }
    }
}
