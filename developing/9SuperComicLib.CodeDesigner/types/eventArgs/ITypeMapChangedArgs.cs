using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ITypeMapChangedArgs : EventArgs
    {
        public readonly ITypeMap Previous;
        public readonly ITypeMap Current;
        public bool AutoDisposing;

        internal ITypeMapChangedArgs(ITypeMap prev, ITypeMap cur)
        {
            AutoDisposing = true;
            Previous = prev;
            Current = cur;
        }
    }
}
