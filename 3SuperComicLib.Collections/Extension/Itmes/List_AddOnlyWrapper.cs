using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal readonly struct List_AddOnlyWrapper<T> : IAddOnlyList<T>
    {
        private readonly IList<T> m_value;

        public List_AddOnlyWrapper(IList<T> list) => m_value = list;

        void IAddOnlyList<T>.Add(T item) => m_value.Add(item);
    }
}
