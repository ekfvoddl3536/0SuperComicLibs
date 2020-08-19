using System;

namespace SuperComicLib.XPatch
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ILBranchableAttribute : Attribute
    {
    }
}
