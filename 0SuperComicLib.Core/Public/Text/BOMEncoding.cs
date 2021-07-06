using System.Text;

namespace SuperComicLib.Text
{
    /// <summary>
    /// with out BOM (byte order mask) encoding
    /// </summary>
    public static class BOMEncoding
    {
        public static readonly Encoding UTF8 = new UTF8Encoding(false, false);
        public static readonly Encoding Unicode = new UnicodeEncoding(false, false, false);
        public static readonly Encoding BigEndianUnicode = new UnicodeEncoding(true, false, false);
    }
}
