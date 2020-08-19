using System.Collections;

namespace SuperComicLib.Collections
{
    public static class ICollectionExtension
    {
        public static IUniObserver GetObserver(this ICollection collection) => new CountObserver(collection);
    }
}
