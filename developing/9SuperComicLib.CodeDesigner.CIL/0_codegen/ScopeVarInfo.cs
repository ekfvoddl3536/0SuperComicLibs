using System;
using System.Reflection.Emit;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ScopeVarInfo
    {
        // index < 0 = arg
        // index >= 0 = local
        public readonly Type type;
        public readonly int index;

        public ScopeVarInfo(Type type, int index)
        {
            this.type = type;
            this.index = index;
        }

#if DEBUG
        // 논리 참고용
        public bool IsLocal => index >= 0;

        public int FixIndex => index & int.MaxValue;
#endif
    }

    public sealed class GlobalVarInfo
    {
        // class는 타입으로 처리해야된다
        // 경우의 수는 단 2가지
        public readonly Type id; // using <id> 용
        public readonly FieldBuilder fld; // 필드

        public GlobalVarInfo(Type id) => this.id = id;

        public GlobalVarInfo(FieldBuilder fld) => this.fld = fld;

#if DEBUG
        // 논리 참고용
        public bool IsField => id == null;
#endif
    }
}
