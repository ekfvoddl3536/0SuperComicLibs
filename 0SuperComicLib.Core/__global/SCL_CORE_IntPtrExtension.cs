using System;
using System.Runtime.InteropServices;

public static class SCL_CORE_IntPtrExtension
{
    public static object Cast(this IntPtr target)
    {
        GCHandle gchnd = GCHandle.FromIntPtr(target);
        object result = gchnd.Target;
        gchnd.Free();
        return result;
    }

    public static T Cast<T>(this IntPtr target)
    {
        GCHandle gchnd = GCHandle.FromIntPtr(target);
        if (typeof(T) != gchnd.Target.GetType())
        {
            gchnd.Free();
            throw new InvalidOperationException();
        }
        else
        {
            T result = (T)gchnd.Target;
            gchnd.Free();
            return result;
        }
    }

    public static T CastUnsafe<T>(this IntPtr target)
    {
        GCHandle gchnd = GCHandle.FromIntPtr(target);
        T result = (T)gchnd.Target;
        gchnd.Free();
        return result;
    }
}