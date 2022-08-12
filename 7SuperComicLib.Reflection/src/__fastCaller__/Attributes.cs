using System;

namespace SuperComicLib.Runtime
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TargetFieldAttribute : Attribute
    {
        internal readonly string name;
        internal readonly bool refm;

        public TargetFieldAttribute(bool refMode) : this(null, refMode) { }

        public TargetFieldAttribute(string fieldName) : this(fieldName, false) { }

        public TargetFieldAttribute(string fieldName, bool refMode)
        {
            name = fieldName;
            refm = refMode;
        }
    }

    /// <summary>
    /// Optional
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class TargetFunctionAttribute : Attribute
    {
        internal readonly string name;

        public TargetFunctionAttribute(string funcName) => name = funcName;
    }
}
