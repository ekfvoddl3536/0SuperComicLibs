using System;
using System.IO;
using System.Text;
using SuperComicLib.IO;
using SuperComicLib.Text;

namespace SuperComicLib.CodeDesigner
{
    public abstract class PreProcessor : IDisposable
    {
        public CStreamReader Convert(Stream stream) => Convert(stream, Encoding.Default, true);

        public CStreamReader Convert(Stream stream, Encoding encoding) => Convert(stream, encoding, true);

        public CStreamReader Convert(Stream stream, Encoding encoding, bool leaveOpen)
        {
            if (stream == null || encoding == null)
                throw new ArgumentNullException(stream == null ? nameof(stream) : nameof(encoding));

            CStreamReader input = new CStreamReader(stream, encoding, true, 1024, leaveOpen);

            StringStream ss = new StringStream(1024);
            CStreamReader result = new CStreamReader(ss, encoding, true, 1024, false);

            StreamWriter writer = ss.In();
            OnConvert(input, writer);
            writer.Dispose();

            input.Dispose();

            return result;
        }

        protected abstract void OnConvert(CStreamReader input, StreamWriter output);

        protected internal virtual void OnSerialize(BinaryWriter writer) { }
        protected internal virtual void OnDeserialize(BinaryReader reader) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
