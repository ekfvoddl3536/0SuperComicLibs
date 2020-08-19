using System;

namespace SuperComicLib.DataObject
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigAttribute : Attribute
    {
        internal readonly string rel_name;

        public ConfigAttribute() { }
        public ConfigAttribute(string rel_filename_without_extension) => rel_name = rel_filename_without_extension;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MarkAsNameAttribute : Attribute
    {
        internal readonly string opt_name;

        public MarkAsNameAttribute(string opt_name) => this.opt_name = opt_name ?? throw new ArgumentNullException(nameof(opt_name));
    }
}
