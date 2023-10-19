// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using SuperComicLib.CodeContracts;

/// <summary>
/// 동일한 크기의 두 배열을 동시에 열거하는 작업에 대한 for 본문 대리자
/// </summary>
/// <typeparam name="T1">첫번째 배열의 타입 원소</typeparam>
/// <typeparam name="T2">두번째 배열의 타입 원소</typeparam>
/// <param name="item1">첫번째 배열의 현재 원소 값에 대한 참조</param>
/// <param name="item2">두번째 배열의 현재 원소 값에 대한 참조</param>
/// <param name="index">현재 배열의 인덱스</param>
public delegate void ForEachTupleAction<T1, T2>(ref T1 item1, ref T2 item2, int index);

/// <summary>
/// 광역(global)으로 사용될 수 있는 배열 관련 편의 기능 확장입니다.
/// </summary>
public static class SUPERCOMICLIB_CORE__ArrayExtension__
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

    /// <summary>
    /// 부분 배열을 만듭니다.
    /// </summary>
    /// <typeparam name="T">배열의 원소 타입</typeparam>
    /// <param name="_arr">원본 배열</param>
    /// <param name="startIdx">원본 배열에서 읽기 시작할 위치(지정된 위치의 원소 포함)</param>
    /// <param name="count">읽을 원소의 개수</param>
    /// <returns>원본 배열에 대한, 부분 배열</returns>
    [return: NotNull]
    public static T[] SubArray<T>(this T[] _arr, int startIdx, int count)
    {
        if (count == 0)
            return Array.Empty<T>();

        var result = new T[count];
        Array.Copy(_arr, startIdx, result, 0, count);

        return result;
    }

    /// <summary>
    /// 부분 배열 만들기를 시도합니다.
    /// </summary>
    /// <typeparam name="T">배열의 원소 타입</typeparam>
    /// <param name="_arr">원본 배열</param>
    /// <param name="startIdx">원본 배열에서 읽기 시작할 위치(지정된 위치의 원소 포함)</param>
    /// <param name="count">읽을 원소의 개수</param>
    /// <param name="result">부분 배열의 결과 (nullable)</param>
    /// <returns>성공했을 경우 true</returns>
    public static bool TrySubArray<T>(this T[] _arr, int startIdx, int count, out T[] result)
    {
        if (_arr == null || (startIdx | count) < 0 || (uint)(startIdx + count) > (uint)_arr.Length)
            goto _false;

        if (count == 0)
        {
            result = Array.Empty<T>();
            return true;
        }

        result = new T[count];
        Array.Copy(_arr, startIdx, result, 0, count);

        return true;

    _false:
        result = null;
        return false;
    }

    /// <summary>
    /// 배열 내 지정된 모든 원소 값을 새로운 값으로 대체합니다.
    /// </summary>
    /// <typeparam name="T">배열의 원소 타입</typeparam>
    /// <param name="_arr">원본 배열</param>
    /// <param name="old">검색할 값</param>
    /// <param name="replace">대체할 값</param>
    /// <returns>원본 배열의 복사본에 대해 연산을 수행한 결과를 저장한 배열</returns>
    public static T[] ReplaceAll<T>(this T[] _arr, T old, T replace) where T : IEquatable<T>
    {
        T[] result = new T[_arr.Length];

        for (int x = 0; x < _arr.Length; x++)
        {
            ref T now = ref _arr[x];
            result[x] = now.Equals(old) ? replace : now;
        }

        return result;
    }

    /// <summary>
    /// 배열 내 지정된 모든 원소 값 하나를 새로운 값 배열로 대체합니다.
    /// </summary>
    /// <typeparam name="T">배열의 원소 타입</typeparam>
    /// <param name="_arr">원본 배열</param>
    /// <param name="old">검색할 값</param>
    /// <param name="replace">대체할 값 배열</param>
    /// <returns>원본 배열의 복사본에 대해 연산을 수행한 결과를 저장한 배열</returns>
    public static T[] ReplaceAll<T>(this T[] _arr, T old, IEnumerable<T> replace) where T : IEquatable<T>
    {
        List<T> result = new List<T>();
        for (int x = 0; x < _arr.Length; x++)
        {
            T now = _arr[x];
            if (now.Equals(old))
                result.AddRange(replace);
            else
                result.Add(now);
        }
        return result.ToArray();
    }

    /// <summary>
    /// 배열 내 지정된 모든 원소 값 배열을 새로운 값 배열로 대체합니다.
    /// </summary>
    /// <typeparam name="T">배열의 원소 타입</typeparam>
    /// <param name="_arr">원본 배열</param>
    /// <param name="old">검색할 값 배열</param>
    /// <param name="replace">대체할 값 배열</param>
    /// <returns>원본 배열의 복사본에 대해 연산을 수행한 결과를 저장한 배열</returns>
    public static T[] ReplaceAll<T>(this T[] _arr, T[] old, T[] replace) where T : IEquatable<T>
    {
        var result = new List<T>();

        for (int x = 0; x + replace.Length <= _arr.Length;)
        {
            for (int i = 0; i < old.Length; i++)
                if (_arr[x + i].Equals(old[i]) == false)
                    goto _matchFalse;

            for (int i = 0; i < replace.Length; i++)
                result.Add(replace[i]);

            x += replace.Length;
            continue;

        _matchFalse:
            result.Add(_arr[x]);
            x++;
        }

        return result.ToArray();
    }

    /// <summary>
    /// 배열의 값을 replace합니다
    /// </summary>
    /// <param name="_arr">원본 배열</param>
    /// <param name="old">찾을 값 입니다</param>
    /// <param name="replace">새 값 입니다</param>
    /// <param name="indexes">old를 찾은 가장 위치들을 저장합니다</param>
    public static T[] ReplaceAll<T>(this T[] _arr, T old, T replace, out int[] indexes) where T : IEquatable<T>
    {
        var result = new T[_arr.Length];
        var idxs = new List<int>();

        for (int x = 0; x < _arr.Length; x++)
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
    /// <param name="_arr">원본 배열</param>
    /// <param name="old">찾을 값 입니다</param>
    /// <param name="replace">새 값 입니다</param>
    /// <param name="indexes">old를 찾은 가장 위치들을 저장합니다</param>
    public static T[] ReplaceAll<T>(this T[] _arr, T old, IEnumerable<T> replace, out int[] indexes) where T : IEquatable<T>
    {
        var result = new List<T>();
        var idxs = new List<int>();

        for (int x = 0; x < _arr.Length; x++)
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
    /// <param name="_arr">원본 배열</param>
    /// <param name="old">찾을 값 입니다</param>
    /// <param name="replace">새 값 입니다</param>
    /// <param name="indices">old를 찾은 가장 위치들을 저장합니다</param>
    public static T[] ReplaceAll<T>(this T[] _arr, T[] old, T[] replace, out int[] indices) where T : IEquatable<T>
    {
        var idxs = new List<int>();
        var result = new List<T>();

        for (int x = 0; x + replace.Length <= _arr.Length;)
        {
            for (int i = 0; i < old.Length; i++)
                if (_arr[x + i].Equals(old[i]) == false)
                    goto _matchFalse;

            for (int i = 0; i < replace.Length; i++)
                result.Add(replace[i]);

            idxs.Add(x);
            x += replace.Length;
            continue;

        _matchFalse:
            result.Add(_arr[x]);
            x++;
        }

        indices = idxs.ToArray();
        return result.ToArray();
    }

    /// <summary>
    /// 동일한 크기의 배열 두 개를 동시에 열거합니다.
    /// </summary>
    /// <typeparam name="T1">첫번째 배열 원소의 타입</typeparam>
    /// <typeparam name="T2">두번째 배열 원소의 타입</typeparam>
    /// <param name="_arr1">첫번째 배열 원본</param>
    /// <param name="_arr2">두번째 배열 원본</param>
    /// <param name="action">본문 대리자</param>
    public static void ForEachTuple<T1, T2>(this T1[] _arr1, T2[] _arr2, ForEachTupleAction<T1, T2> action)
    {
        if (_arr1 == null)
            throw new ArgumentNullException(nameof(_arr1));

        if (_arr1 == null)
            throw new ArgumentNullException(nameof(_arr2));

        for (int i = 0; i < _arr1.Length && i < _arr2.Length; i++)
            action.Invoke(ref _arr1[i], ref _arr2[i], i);
    }
}
