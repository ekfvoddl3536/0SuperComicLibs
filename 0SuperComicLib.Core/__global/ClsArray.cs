using System;
using System.Collections.Generic;

public static class ClsArray
{
    public static void DisposeAll<T>(ref T[] _arr) where T : class
    {
        for (int x = _arr.Length - 1; x >= 0; x--)
        {
            ref T item = ref _arr[x];
            if (item is IDisposable res)
                res.Dispose();
            item = null;
        }
        _arr = null;
    }

    public static void DisposeAll<T>(ref List<T> _list) where T : class
    {
        foreach (T a in _list)
            if (a is IDisposable res) 
                res.Dispose();
        _list.Clear();
        _list = null;
    }
}
