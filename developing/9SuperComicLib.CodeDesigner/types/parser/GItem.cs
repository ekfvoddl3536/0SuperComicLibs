using System;
namespace SuperComicLib.CodeDesigner
{
    public class GItem : IEquatable<GItem>
    {
        public readonly int produce;
        public readonly ExpressInt express;

        public GItem(int produce, ExpressInt express)
        {
            this.produce = produce;
            this.express = express;
        }

        public bool Equals(GItem other) =>
            produce == other.produce &&
            express.Equals(other.express);

#if DEBUG
        public override string ToString() =>
            $"{produce} -> {express}";
#endif
    }
}
