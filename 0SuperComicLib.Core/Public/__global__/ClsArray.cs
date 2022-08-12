using System;
using System.Collections.Generic;

namespace SuperComicLib
{
    public static class ClsArray
    {
        public static void DeleteAll<T>(ref T[] _arr) where T : class
        {
            for (int x = _arr.Length - 1; x >= 0; x--)
                _arr[x] = null;

            _arr = null;
        }

        public static void DisposeAll<T>(ref T[] _arr) where T : class, IDisposable
        {
            for (int x = _arr.Length - 1; x >= 0; x--)
            {
                ref T item = ref _arr[x];
                item.Dispose();
                item = null;
            }
            _arr = null;
        }

        public static void DisposeAll<T>(ref List<T> _list) where T : class, IDisposable
        {
            for (int i = _list.Count; --i >= 0;)
                _list[i].Dispose();

            _list.Clear();
            _list = null;
        }
    }
}