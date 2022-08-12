using System.Text;

namespace SuperComicLib.Text
{
    public sealed class NopEncoding : Encoding
    {
        public static readonly NopEncoding INSTANCE = new NopEncoding();

        private NopEncoding() { }

        public override int GetByteCount(char[] chars, int index, int count) => 0;
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => 0;
        public override int GetCharCount(byte[] bytes, int index, int count) => 0;
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => 0;
        public override int GetMaxByteCount(int charCount) => 0;
        public override int GetMaxCharCount(int byteCount) => 0;
    }
}
