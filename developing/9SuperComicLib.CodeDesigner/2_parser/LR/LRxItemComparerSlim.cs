using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class LRxItemComparerSlim : IEqualityComparer<LRxItem>
    {
        internal static readonly IEqualityComparer<LRxItem> Instance = new LRxItemComparerSlim();

        private LRxItemComparerSlim() { }

        bool IEqualityComparer<LRxItem>.Equals(LRxItem x, LRxItem y) => x.CoreEquals(y);

        int IEqualityComparer<LRxItem>.GetHashCode(LRxItem obj) => obj.GetHashCode();
    }
}
