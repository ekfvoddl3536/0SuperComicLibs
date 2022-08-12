using System;
using SuperComicLib.Core;

namespace SuperComicLib.Runtime
{
    [Flags]
    public enum ExceptionBlockType
    {
        Begin = 0x80,
        End = 0x40,

        None = 0,
        BigBlock = 0x1,
        FilterBlock = 0x2,
        FinallyBlock = 0x4,
        CatchBlock = 0x8,
        FaultBlock = 0x10,
    }

    public readonly struct ExceptionBlockInfo
    {
        public static readonly ExceptionBlockInfo Empty;

        public readonly ExceptionBlockType blockType;
        public readonly Type catchType;

        public bool IsValid => blockType > 0;

        public ExceptionBlockInfo(ExceptionBlockType blockType, Type catchType)
        {
            this.blockType = blockType;
            this.catchType = catchType;
        }

        public ExceptionBlockInfo(ExceptionBlockType blockType) : this(blockType, null) { }

        public override bool Equals(object obj) => false;
        public override int GetHashCode()
        {
            int result = IntHash.Combine(7, (int)blockType);
            return
                catchType != null 
                ? IntHash.Combine(result, catchType.GetHashCode()) 
                : result;
        }

        public static bool operator ==(ExceptionBlockInfo left, ExceptionBlockInfo right) =>
            left.blockType == right.blockType &&
            left.catchType == right.catchType;

        public static bool operator !=(ExceptionBlockInfo left, ExceptionBlockInfo right) =>
            left.blockType != right.blockType ||
            left.catchType != right.catchType;
    }
}
