namespace SuperComicLib.IO
{
    public static unsafe class NativeCharStreamExtensions
    {
        public static int IndexOfWhiteSpace(this NativeCharStream @this, int array_index, int array_length)
        {
            char* src = @this._source + array_index;
            char* end = @this._source + array_length;

            while (src != end && !char.IsWhiteSpace(*src))
                src++;

            return (int)(src - @this._source);
        }

        public static int IndexOfDigit(this NativeCharStream @this, int array_index, int array_length)
        {
            char* src = @this._source + array_index;
            char* end = @this._source + array_length;

            while (src != end && !char.IsDigit(*src))
                src++;

            return (int)(src - @this._source);
        }

        public static int IndexOfLetter(this NativeCharStream @this, int array_index, int array_length)
        {
            char* src = @this._source + array_index;
            char* end = @this._source + array_length;

            while (src != end && !char.IsLetter(*src))
                src++;

            return (int)(src - @this._source);
        }

        public static int IndexOfLetterOrDigit(this NativeCharStream @this, int array_index, int array_length)
        {
            char* src = @this._source + array_index;
            char* end = @this._source + array_length;

            while (src != end && !char.IsLetterOrDigit(*src))
                src++;

            return (int)(src - @this._source);
        }
    }
}
