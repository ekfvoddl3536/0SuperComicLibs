using System;

namespace SuperComicWorld
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MarkEmptyMethodBodyAttribute : Attribute
    {
    }
}
