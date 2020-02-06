using System;
using System.Collections.Generic;
using System.Linq;

public unsafe delegate void IteratorLoopHandler<T>(SuperComicLib.Iterator<T> _iterator) where T : unmanaged;
public unsafe delegate void ReverseIteratorLoopHandler<T>(SuperComicLib.ReverseIterator<T> _iterator) where T : unmanaged;
/// <summary>
/// 비관리형식 배열을 포인터로 읽습니다
/// </summary>
/// <typeparam name="T">관리되지 않는 형식입니다</typeparam>
/// <param name="original">배열 원본입니다</param>
/// <param name="current">현재 데이터 입니다, 같습니다 *(T* + x)</param>
/// <param name="position">배열에서 current가 존재하는 위치입니다</param>
/// <param name="length">배열의 전체 길이입니다</param>
/// <returns>1을 반환하면 다음 데이터를 읽으려고 시도할 것입니다</returns>
public unsafe delegate int ArrayPtrLoopHandler<T>(T* original, ref T current, int position, int length) where T : unmanaged;

public static class ArrayExtension
{
    /// <summary>
    /// 배열에서 첫번째로 발견되는 값의 index를 가져옵니다
    /// </summary>
    /// <returns>값을 찾지 못한 경우 -1이 반환됩니다</returns>
    public static int FirstIndex<T>(this T[] _arr, T value) where T : class
    {
        for (int x = 0, len = _arr.Length; x < len; x++)
            if (_arr[x] == value)
                return x;
        return -1;
    }

    /// <summary>
    /// 배열에서 첫번째로 발견되는 값의 index를 가져옵니다
    /// </summary>
    /// <returns>값을 찾지 못한 경우 -1이 반환됩니다</returns>
    public static int FirstEqualsIndex<T>(this T[] _arr, T value) where T : IEquatable<T>
    {
        for (int x = 0, len = _arr.Length; x < len; x++)
            if (_arr[x].Equals(value))
                return x;
        return -1;
    }

    /// <summary>
    /// 배열에서 가장 마지막으로 발견되는 값의 index를 가져옵니다
    /// </summary>
    /// <returns>값을 찾지 못한 경우 -1이 반환됩니다</returns>
    public static int LastIndex<T>(this T[] _arr, T value) where T : class
    {
        for (int x = _arr.Length - 1; x >= 0; x--)
            if (_arr[x] == value)
                return x;
        return -1;
    }

    /// <summary>
    /// 배열에서 가장 마지막으로 발견되는 값의 index를 가져옵니다
    /// </summary>
    /// <returns>값을 찾지 못한 경우 -1이 반환됩니다</returns>
    public static int LastEqualsIndex<T>(this T[] _arr, T value) where T : IEquatable<T>
    {
        for (int x = _arr.Length - 1; x >= 0; x--)
            if (_arr[x].Equals(value))
                return x;
        return -1;
    }

    public static T[] SubArray<T>(this T[] _arr, int startIdx, int endIdx)
    {
        if (_arr.Length < endIdx)
            throw new ArgumentOutOfRangeException(nameof(endIdx));

        int count = endIdx - startIdx;
        if (count <= 0)
            throw new ArgumentOutOfRangeException();

        T[] result = new T[count];
        for (int x = 0; x < count; x++, startIdx++)
            result[x] = _arr[startIdx];
        return result;
    }

    public static bool TrySubArray<T>(this T[] _arr, int startIdx, int endIdx, out T[] result)
    {
        if (_arr.Length < endIdx)
        {
            result = null;
            return false;
        }

        int count = endIdx - startIdx;
        if (count <= 0)
        {
            result = null;
            return false;
        }

        result = new T[count];
        for (int x = 0; x < count; x++, startIdx++)
            result[x] = _arr[startIdx];
        return true;
    }

    public static T[] ReplaceAll<T>(this T[] _arr, T old, T replace) where T : IEquatable<T>
    {
        int len = _arr.Length;
        T[] result = new T[len];
        for (int x = 0; x < len; x++)
        {
            T now = _arr[x];
            result[x] = now.Equals(old) ? replace : now;
        }
        return result;
    }

    public static T[] ReplaceAll<T>(this T[] _arr, T old, T[] replace) where T : IEquatable<T>
    {
        List<T> result = new List<T>();
        for (int x = 0, len = _arr.Length; x < len; x++)
        {
            T now = _arr[x];
            if (now.Equals(old))
                result.AddRange(replace);
            else
                result.Add(now);
        }
        return result.ToArray();
    }

    public static T[] ReplaceAll<T>(this T[] _arr, T[] old, T[] replace) where T : IEquatable<T>
    {
        List<T> result = new List<T>();
        int len = old.Length,
            rlen = replace.Length;
        for (int x = 0, max = _arr.Length; x + rlen <= max;)
        {
            bool matched = true;
            for (int i = 0; i < len; i++)
                if (_arr[x + i].Equals(old[i]) == false)
                {
                    matched = false;
                    break;
                }
            if (matched)
            {
                for (int i = 0; i < rlen; i++)
                    result.Add(replace[i]);
                x += len;
            }
            else
            {
                result.Add(_arr[x]);
                x++;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// 배열의 값을 replace합니다
    /// </summary>
    /// <param name="old">찾을 값 입니다</param>
    /// <param name="replace">새 값 입니다</param>
    /// <param name="indexes">old를 찾은 가장 위치들을 저장합니다</param>
    public static T[] ReplaceAll<T>(this T[] _arr, T old, T replace, out int[] indexes) where T : IEquatable<T>
    {
        int len = _arr.Length;
        T[] result = new T[len];
        List<int> idxs = new List<int>();
        for (int x = 0; x < len; x++)
        {
            T now = _arr[x];
            if (now.Equals(old))
            {
                idxs.Add(x);
                result[x] = replace;
            }
            else
                result[x] = now;
        }
        indexes = idxs.ToArray();
        return result;
    }

    /// <summary>
    /// 배열의 값을 replace합니다
    /// </summary>
    /// <param name="old">찾을 값 입니다</param>
    /// <param name="replace">새 값 입니다</param>
    /// <param name="indexes">old를 찾은 가장 위치들을 저장합니다</param>
    public static T[] ReplaceAll<T>(this T[] _arr, T old, T[] replace, out int[] indexes) where T : IEquatable<T>
    {
        List<T> result = new List<T>();
        List<int> idxs = new List<int>();
        for (int x = 0, len = _arr.Length; x < len; x++)
        {
            T now = _arr[x];
            if (now.Equals(old))
            {
                result.AddRange(replace);
                idxs.Add(x);
            }
            else
                result.Add(now);
        }
        indexes = idxs.ToArray();
        return result.ToArray();
    }

    /// <summary>
    /// 배열의 값을 replace합니다
    /// </summary>
    /// <param name="old">찾을 값 입니다</param>
    /// <param name="replace">새 값 입니다</param>
    /// <param name="indexes">old를 찾은 가장 위치들을 저장합니다</param>
    public static T[] ReplaceAll<T>(this T[] _arr, T[] old, T[] replace, out int[] indexes) where T : IEquatable<T>
    {
        List<T> result = new List<T>();
        List<int> idxs = new List<int>();
        int len = old.Length,
            rlen = replace.Length;
        for (int x = 0, max = _arr.Length; x + rlen <= max;)
        {
            bool matched = true;
            for (int i = 0; i < len; i++)
                if (_arr[x + i].Equals(old[i]) == false)
                {
                    matched = false;
                    break;
                }
            if (matched)
            {
                for (int i = 0; i < rlen; i++)
                    result.Add(replace[i]);
                idxs.Add(x);
                x += len;
            }
            else
            {
                result.Add(_arr[x]);
                x++;
            }
        }
        indexes = idxs.ToArray();
        return result.ToArray();
    }

    /// <summary>
    /// 배열의 값을 replace합니다
    /// </summary>
    /// <param name="oldLength">각 indexes 위치부터 무시할 배열의 크기입니다</param>
    /// <param name="indexes">배열 내에서 replace할 데이터가 있는 위치들 입니다</param>
    /// <param name="replace">새 값입니다</param>
    public static T[] ReplaceAllFast<T>(this T[] _arr, int oldLength, int[] indexes, T[] replace) where T : IEquatable<T>
    {
        List<T> result = new List<T>();
        int
            max = _arr.Length,
            x = 0;

        foreach (int idx in indexes)
            if (x == idx)
            {
                result.AddRange(replace);
                x += oldLength;
            }
            else
            {
                result.Add(_arr[x]);
                x++;
            }

        for (; x < max; x++)
            result.Add(_arr[x]);

        return result.ToArray();
    }

    public static unsafe void Begin_S<T>(this T[] _arr, IteratorLoopHandler<T> handler) where T : unmanaged
    {
        if (handler == null)
            return;
        fixed (T* ptr = _arr)
            handler.Invoke(new SuperComicLib.Iterator<T>(ptr, _arr.Length));
    }

    public static unsafe void Begin<T>(this T[] _arr, IteratorLoopHandler<T> handler) where T : unmanaged
    {
        fixed (T* ptr = _arr)
            handler.Invoke(new SuperComicLib.Iterator<T>(ptr, _arr.Length));
    }

    public static unsafe void Begin<T>(this T[] _arr, ArrayPtrLoopHandler<T> handler) where T : unmanaged
    {
        fixed (T* ptr = _arr)
            for (int x = 0, max = _arr.Length; x < max;)
                x += handler.Invoke(ptr, ref ptr[x], x, max);
    }

    public static unsafe void RBegin_S<T>(this T[] _arr, ReverseIteratorLoopHandler<T> handler) where T : unmanaged
    {
        if (handler == null)
            return;
        fixed (T* ptr = _arr)
            handler.Invoke(new SuperComicLib.ReverseIterator<T>(ptr, _arr.Length));
    }

    public static unsafe void RBegin<T>(this T[] _arr, ReverseIteratorLoopHandler<T> handler) where T : unmanaged
    {
        fixed (T* ptr = _arr)
            handler.Invoke(new SuperComicLib.ReverseIterator<T>(ptr, _arr.Length));
    }

    public static unsafe void RBegin<T>(this T[] _arr, ArrayPtrLoopHandler<T> handler) where T : unmanaged
    {
        fixed (T* ptr = _arr)
            for (int x = _arr.Length, max = x; x >= 0;)
                x -= handler.Invoke(ptr, ref ptr[x], x, max);
    }

    public static string ToString<T>(this IEnumerable<T> _arr, Func<T, string> converter = null, string value = " ")
    {
        if (converter == null)
            converter = cur => cur.ToString();
        string v = _arr.Aggregate(string.Empty, (p, c) => $"{p}{(p.Length != 0 ? value : string.Empty)}{converter.Invoke(c)}");
        return v;
    }
}
