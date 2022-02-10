using System;

namespace SuperComicLib.CodeDesigner
{
    public sealed class GItem : IEquatable<GItem>
    {
        public readonly int produce;
        public readonly ExpressInt express;

        public GItem(int produce, ExpressInt express)
        {
            this.produce = produce | ExpressInt.nonterminal_flag;
            this.express = express;
        }

        public bool Equals(GItem other) =>
            produce == other.produce &&
            express.Equals(other.express);

#if DEBUG
        public override string ToString() =>
            $"{Grammar.bag[produce]} -> {express}";
#endif
    }
}
