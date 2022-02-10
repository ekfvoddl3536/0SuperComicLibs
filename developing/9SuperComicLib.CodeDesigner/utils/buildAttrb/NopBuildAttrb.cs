namespace SuperComicLib.CodeDesigner
{
    internal sealed class NopBuildAttrb : IBuildAttribute
    {
        public static readonly IBuildAttribute Instance = new NopBuildAttrb();

        private NopBuildAttrb() { }

        public void AddImplement(string uri) { }
    }
}
