namespace SuperComicLib.LowLevel
{
    public static unsafe class Extensions
    {
        public static byte[] GetBytes<T>(ref this T value) where T : unmanaged
        {
            byte[] res = new byte[sizeof(T)];
            fixed (byte* ptr = res)
                *(T*)ptr = value;
            return res;
        }

        public static T GetValue<T>(this byte[] bytes) where T : unmanaged
        {
            fixed (byte* ptr = bytes)
                return *(T*)ptr;
        }

        public static T GetValue<T>(this byte[] bytes, int idx) where T : unmanaged
        {
            fixed (byte* ptr = bytes)
                return *(T*)(ptr + idx);
        }

        public static T GetValue_s<T>(this byte[] bytes) where T : unmanaged
        {
            if (sizeof(T) != bytes.Length)
                return default;
            fixed (byte* ptr = bytes)
                return *(T*)ptr;
        }

        public static T GetValue_s<T>(this byte[] bytes, int idx) where T : unmanaged
        {
            if (sizeof(T) + idx != bytes.Length)
                return default;
            fixed (byte* ptr = bytes)
                return *(T*)(ptr + idx);
        }

        public static ref T RefValue<T>(this byte[] bytes, int idx) where T : unmanaged
        {
            fixed (byte* ptr = bytes)
                return ref *(T*)(ptr + idx);
        }

        public static TOut GetValue_Uni<TIn, TOut>(this TIn[] arr) 
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return *(TOut*)ptr;
        }

        public static TOut GetValue_Uni<TIn, TOut>(this TIn[] arr, int byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return *(TOut*)((byte*)ptr + byte_offset);
        }

        public static TOut GetValue_Uni<TIn, TOut>(this TIn[] arr, long byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return *(TOut*)((byte*)ptr + byte_offset);
        }

        public static TOut GetValue_Uni<TIn, TOut>(this TIn[] arr, int start_arrayIdx, int byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return *(TOut*)((byte*)(ptr + start_arrayIdx) + byte_offset);
        }

        public static TOut GetValue_Uni<TIn, TOut>(this TIn[] arr, int start_arrayIdx, long byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return *(TOut*)((byte*)(ptr + start_arrayIdx) + byte_offset);
        }

        public static ref TOut RefValue_Uni<TIn, TOut>(this TIn[] arr, int byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return ref *(TOut*)((byte*)ptr + byte_offset);
        }

        public static ref TOut RefValue_Uni<TIn, TOut>(this TIn[] arr, long byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return ref *(TOut*)((byte*)ptr + byte_offset);
        }

        public static ref TOut RefValue_Uni<TIn, TOut>(this TIn[] arr, int start_arrayIdx, int byte_offset)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return ref *(TOut*)((byte*)(ptr + start_arrayIdx) + byte_offset);
        }

        public static ref TOut RefValue_Uni<TIn, TOut>(this TIn[] arr, int start_arrayIdx, long byte_offset)
           where TIn : unmanaged
           where TOut : unmanaged
        {
            fixed (TIn* ptr = arr)
                return ref *(TOut*)((byte*)(ptr + start_arrayIdx) + byte_offset);
        }
    }
}
