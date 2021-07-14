using System;

namespace SuperComicWorld
{
    /// <summary>
    /// Component Attribute 기본
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class FieldBaseAttribute : Attribute
    {
    }
}