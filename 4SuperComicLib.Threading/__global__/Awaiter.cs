using System.Threading;

namespace SuperComicLib.Threading
{
    public readonly struct Awaiter
    {
        private readonly AutoResetEvent wh;

        internal Awaiter(AutoResetEvent wh) => this.wh = wh;

        public void Wait() => wh.WaitOne();

        public override bool Equals(object obj) => obj is Awaiter k && k == this;
        public override int GetHashCode() => wh.GetHashCode();

        public static bool operator ==(Awaiter left, Awaiter right) => left.wh == right.wh;
        public static bool operator !=(Awaiter left, Awaiter right) => left.wh != right.wh;
    }
}
