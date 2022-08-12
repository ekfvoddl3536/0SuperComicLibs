using SuperComicLib.CodeContracts;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace SuperComicLib.IO
{
    public static unsafe class FormatStream
    {
        [return: NotNull]
        public static FormatStreamReader<TResult, TLocalBuf> UnsafeFastNew<TResult, TLocalBuf>(
            [DisallowNull] IFormatParseResolver<TResult, TLocalBuf> resolver,
            [DisallowNull] Stream stream,
            [AllowNull] Encoding encoding = null,
            int bufferSize = 0)
            where TResult : unmanaged
            where TLocalBuf : unmanaged
        {
            Contract.Requires(stream != null);
            Contract.Requires(resolver != null);
            Contract.Requires(bufferSize > 16);

            if (encoding == null)
                encoding = Encoding.Default;

            bufferSize = (int)CMath.Max((uint)resolver.EncodingBufferSize, 128u);

            return
                new FormatStreamReader<TResult, TLocalBuf>(
                    stream,
                    encoding,
                    resolver,
                    bufferSize);
        }
    }
}
