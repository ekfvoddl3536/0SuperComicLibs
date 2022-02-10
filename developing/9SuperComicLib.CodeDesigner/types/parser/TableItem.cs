using System;

namespace SuperComicLib.CodeDesigner
{
    public readonly struct TableItem : IEquatable<TableItem>
    {
        public readonly int actType;
        public readonly int nextstate;

        public TableItem(int actType, int nextstate)
        {
            this.actType = actType;
            this.nextstate = nextstate;
        }

        public bool IsInvalid => 
            actType == 0 && 
            nextstate == 0;

        public bool Equals(TableItem other) => this == other;

        public override bool Equals(object obj) =>
            obj is TableItem other && Equals(other);

        public override int GetHashCode() => 21 * (actType + nextstate);

        public static bool operator ==(TableItem left, TableItem right) =>
            left.actType == right.actType &&
            left.nextstate == right.nextstate;
        public static bool operator !=(TableItem left, TableItem right) =>
            left.actType != right.actType ||
            left.nextstate != right.nextstate;
    }
}
