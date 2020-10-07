using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class VarInfo
    {
        public readonly Type type;
        public readonly int index;
        public readonly bool isLocal;

        public VarInfo(Type type, int index, bool isLocal)
        {
            this.type = type;
            this.index = index;
            this.isLocal = isLocal;
        }
    }
}
