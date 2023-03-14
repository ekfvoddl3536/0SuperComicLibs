using System;
using System.Collections.Generic;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Collections
{
    public static unsafe partial class SystemCollectionsGenericEnumerator
    {
        #region aggregate
        public static TResult Aggregate<TSource, TAccumulate, TResult>([DisallowNull] this IEnumerator<TSource> @this, [DisallowNull] TAccumulate seed, [DisallowNull] Func<TAccumulate, TSource, TAccumulate> func, [DisallowNull] Func<TAccumulate, TResult> resultSelector)
        {
            TAccumulate result = seed;
            while (@this.MoveNext())
                result = func.Invoke(result, @this.Current);

            return resultSelector.Invoke(result);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>([DisallowNull] this IEnumerator<TSource> @this, [DisallowNull] TAccumulate seed, [DisallowNull] Func<TAccumulate, TSource, TAccumulate> func)
        {
            TAccumulate result = seed;
            while (@this.MoveNext())
                result = func.Invoke(result, @this.Current);

            return result;
        }

        public static TSource Aggregate<TSource>([DisallowNull] this IEnumerator<TSource> @this, [DisallowNull] Func<TSource, TSource, TSource> func)
        {
            if (!@this.MoveNext())
                return default;

            TSource result = @this.Current;
            while (@this.MoveNext())
                result = func.Invoke(result, @this.Current);

            return result;
        }
        #endregion

        #region all & any
        public static bool All<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] Func<T, bool> predicate)
        {
            while (src.MoveNext())
                if (!predicate.Invoke(src.Current))
                    return false;

            return true;
        }

        public static bool Any<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] Func<T, bool> predicate)
        {
            while (src.MoveNext())
                if (predicate.Invoke(src.Current))
                    return true;

            return false;
        }

        public static bool Any<T>([DisallowNull] this IEnumerator<T> src) => src.MoveNext();
        #endregion

        #region append & concat & count
        public static IEnumerator<T> Append<T>([DisallowNull] this IEnumerator<T> src, [AllowNull] T element) =>
            src is MergeMax4Enumerator<T> mm4e
            ? mm4e.AppendCore1(element)
            : src is ScalableEnumerator<T> ade
            ? ade.AppendCore1(element)
            : new MergeMax4Enumerator<T>(src, new ScalableEnumerator<T>(element));

        public static IEnumerator<T> Concat<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] IEnumerator<T> next) =>
            src is MergeMax4Enumerator<T> mm4e
            ? mm4e.MergeCoreN(next)
            : new MergeMax4Enumerator<T>(src, next);

        public static uint Count<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] Func<T, bool> predicate)
        {
            uint cnt = 0;
            while (src.MoveNext())
                if (predicate.Invoke(src.Current))
                    cnt = checked(cnt + 1);

            return cnt;
        }

        public static uint Count<T>([DisallowNull] this IEnumerator<T> src)
        {
            uint cnt = 0;
            while (src.MoveNext())
                cnt = checked(cnt + 1);

            return cnt;
        }
        #endregion

        #region distinct & distinctby & except & exceptby
        public static IEnumerator<T> Distinct<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] IEqualityComparer<T> comparer) =>
            new SuperComic_DistinctEnumerator<T>(src, comparer);

        public static IEnumerator<T> Distinct<T>([DisallowNull] this IEnumerator<T> src) =>
            new SuperComic_DistinctEnumerator<T>(src, EqualityComparer<T>.Default);

        public static IEnumerator<TSource> DistinctBy<TSource, TKey>([DisallowNull] this IEnumerator<TSource> src, Func<TSource, TKey> keySelector) =>
            new SuperComic_DistinctByEnumerator<TSource, TKey>(src, keySelector, EqualityComparer<TKey>.Default);

        public static IEnumerator<TSource> DistinctBy<TSource, TKey>([DisallowNull] this IEnumerator<TSource> src, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
            new SuperComic_DistinctByEnumerator<TSource, TKey>(src, keySelector, comparer);
        #endregion

        #region prepend
        public static IEnumerator<T> Prepend<T>([DisallowNull] this IEnumerator<T> src, T element) =>
             src is MergeMax4Enumerator<T> mm4e
            ? mm4e.PrependCore1(element)
            : src is ScalableEnumerator<T> ade
            ? ade.PrependCore1(element)
            : new MergeMax4Enumerator<T>(new ScalableEnumerator<T>(element), src);
        #endregion

        #region where
        public static IEnumerator<T> Where<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] Func<T, bool> predicate) =>
            new SuperComic_WhereEnumeratorA00<T>(src, predicate);

        public static IEnumerator<T> Where<T>([DisallowNull] this IEnumerator<T> src, [DisallowNull] Func<T, int, bool> predicate) =>
            new SuperComic_WhereEnumeratorB11<T>(src, predicate);

        public static IEnumerator<T> Reverse<T>([DisallowNull] this IEnumerator<T> src)
        {
            var list = new UnsafeList<T>();
            src.EnumerateToEnd(list);
            return list.GetReverseEnumerator();
        }
        #endregion
    }
}
