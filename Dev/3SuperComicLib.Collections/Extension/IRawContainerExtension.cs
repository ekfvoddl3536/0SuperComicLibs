namespace SuperComicLib.Collections
{
    public static class IRawContainerExtension
    {
        public static IReadOnlyRawContainer<T> AsReadOnly<T>(this IRawContainer<T> container) where T : unmanaged =>
            new ReadOnlyRawContainerWrapper<T>(container);
    }
}
