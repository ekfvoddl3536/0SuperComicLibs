using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class TypeMapChangedArgs : EventArgs
    {
        public readonly ITypeMap Previous;
        public readonly ITypeMap Current;
        public bool AutoDisposing;

        internal TypeMapChangedArgs(ITypeMap prev, ITypeMap cur)
        {
            AutoDisposing = true;
            Previous = prev;
            Current = cur;
        }
    }
}
