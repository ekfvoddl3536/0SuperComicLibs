using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IVersionControlledCollectionExtension
    {
        public static IEnumerator<TSource> GetReadOnlyEnumerator<TType, TSource>(this TType @this)
            where TType : IEnumerable<TSource>, IVersionControlledCollection =>
            new SuperComic_VersionBasedReadOnlyEnumerator<TSource>(@this, @this.GetEnumerator());

        public static IEnumerator<TSource> GetReadOnlyEnumerator<TSource>(this IVersionControlledCollection @this, IEnumerator<TSource> enumerator) =>
            new SuperComic_VersionBasedReadOnlyEnumerator<TSource>(@this, enumerator);
    }
}
