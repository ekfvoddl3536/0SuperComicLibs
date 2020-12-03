using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SuperComicLib.IO
{
    public class CStreamReader : TextReader
    {
        public static int DefaultBufferSize = 1024;
        protected const int MinBufferSize = 128;

        #region field
        protected Stream stream;
        protected Encoding encoding;
        protected Decoder decoder;
        protected byte[] bufbyte;
        protected char[] bufchar;
        protected byte[] preamble;
        protected int charpos;
        protected int charlen;
        protected int bytelen;
        protected int bytepos;
        protected bool detectEncoding;
        protected bool isBlocked;
        protected bool leaveOpen;
        protected bool checkPreamble;

        [NonSerialized]
        protected volatile Task asyncReadTask;
        #endregion

        #region constructor
        protected CStreamReader() { }

        public CStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen)
        {
            if (stream == null || encoding == null)
                throw new ArgumentNullException();
            if (!stream.CanRead)
                throw new ArgumentException();
            if (bufferSize < MinBufferSize)
                bufferSize = MinBufferSize;

            this.stream = stream;
            this.encoding = encoding;
            decoder = encoding.GetDecoder();

            bufbyte = new byte[bufferSize];
            bufchar = new char[encoding.GetMaxCharCount(bufferSize)];

            bytelen = 0;
            bytepos = 0;

            detectEncoding = detectEncodingFromByteOrderMarks;
            preamble = encoding.GetPreamble();
            checkPreamble = preamble.Length > 0;

            isBlocked = false;
            this.leaveOpen = leaveOpen;
        }
        #endregion

        #region property
        public virtual Encoding CurrentEncoding => encoding;

        public virtual Stream BaseStream => stream;

        public bool IsLeaveOpen => leaveOpen;

        public bool EndOfStream
        {
            get
            {
                if (stream == null)
                    Error_ReaderClose();

                return charpos >= charlen && ReadBuffer() == 0;
            }
        }
        #endregion

        #region method
        protected void CheckAsyncTaskInProgress()
        {
            Task t = asyncReadTask;

            if (t != null && !t.IsCompleted)
                throw new InvalidOperationException();
        }


        public void DiscardBufferedData()
        {
            CheckAsyncTaskInProgress();

            bytelen = 0;
            charlen = 0;
            charpos = 0;

            if (encoding != null)
                decoder = encoding.GetDecoder();

            isBlocked = false;
        }

        public override int Peek()
        {
            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            return
                charpos >= charlen && (isBlocked || ReadBuffer() == 0)
                ? -1
                : bufchar[charpos];
        }

        public virtual int Peek(int lookahead)
        {
            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            int npos;
            return
                (npos = charpos + lookahead) >= charlen && (isBlocked || ReadBuffer() == 0)
                ? -1
                : bufchar[npos];
        }

        public virtual int Peek(char[] buffer, int index, int count)
        {
            if (buffer.Length - index < count)
                throw new ArgumentException();

            int vcharpos = charpos;
            int n;
            if (charlen - vcharpos == 0)
            {
                n = ReadBuffer();

                if (n == 0)
                    return -1;
            }

            int charsRead = 0;

            while (count > 0)
            {
                n = charlen - vcharpos;
                if (n == 0)
                    break;

                if (n > count)
                    n = count;

                BlockCopy(bufchar, charsRead, buffer, index + charsRead, n << 1);
                vcharpos += n;

                charsRead += n;
                count -= n;
            }

            return charsRead;
        }

        public virtual void Move() => charpos++;

        public virtual void Move(int count) => charpos += count;

        public override int Read()
        {
            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            return
                charpos >= charlen && ReadBuffer() == 0
                ? -1
                : bufchar[charpos++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer.Length - index < count)
                throw new ArgumentException();

            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            int charsRead = 0;

            bool readToUserBuffer = false;
            while (count > 0)
            {
                int n = charlen - charpos;
                if (n <= 0)
                {
                    n = ReadBuffer(buffer, index + charsRead, count, out readToUserBuffer);

                    if (n == 0)
                        break;
                }

                if (n > count)
                    n = count;

                if (!readToUserBuffer)
                {
                    BlockCopy(bufchar, charsRead, buffer, index + charsRead, n << 1);
                    charpos += n;
                }

                charsRead += n;
                count -= n;

                if (isBlocked)
                    break;
            }

            return charsRead;
        }

        public override string ReadToEnd()
        {
            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            StringBuilder sb = new StringBuilder(charlen - charpos);
            do
            {
                sb.Append(bufchar, charpos, charlen - charpos);
                charpos = charlen;
                ReadBuffer();
            } while (charlen > 0);

            return sb.ToString();
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            if (buffer.Length - index < count)
                throw new ArgumentException();

            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            return base.ReadBlock(buffer, index, count);
        }

        protected void CompressBuffer(int n)
        {
            BlockCopy(bufbyte, n, bufbyte, 0, bytelen - n);
            bytelen -= n;
        }

        protected void DetectEncoding()
        {
            if (bytelen < 2)
                return;

            detectEncoding = false;
            bool changedEncoding = false;
            if (bufbyte[0] == 0xFE && bufbyte[1] == 0xFF)
            {
                // Big Endian

                encoding = new UnicodeEncoding(true, true);
                CompressBuffer(2);
                changedEncoding = true;
            }
            else if (bufbyte[0] == 0xFF && bufbyte[1] == 0xFE)
                // Little Endian
                if (bytelen < 4 || bufbyte[2] != 0 || bufbyte[3] != 0)
                {
                    encoding = new UnicodeEncoding(false, true);
                    CompressBuffer(2);
                    changedEncoding = true;
                }
                else
                {
                    encoding = new UTF32Encoding(false, true);
                    CompressBuffer(4);
                    changedEncoding = true;
                }
            else if (bytelen >= 3 && bufbyte[0] == 0xEF && bufbyte[1] == 0xBB && bufbyte[2] == 0xBF)
            {
                encoding = Encoding.UTF8;
                CompressBuffer(3);
                changedEncoding = true;
            }
            else if (bytelen < 4 && bufbyte[0] == 0 && bufbyte[1] == 0 && bufbyte[2] == 0xFE && bufbyte[3] == 0xFF)
            {
                encoding = new UTF32Encoding(true, true);
                CompressBuffer(4);
                changedEncoding = true;
            }
            else if (bytelen == 2)
                detectEncoding = true;

            if (changedEncoding)
            {
                decoder = encoding.GetDecoder();
                bufchar = new char[encoding.GetMaxCharCount(bufbyte.Length)];
            }
        }

        protected bool IsPreamble()
        {
            if (!checkPreamble)
                return false;

            int len =
                bytelen >= preamble.Length
                ? preamble.Length - bytepos
                : bytelen - bytepos;

            for (int x = 0; x < len; x++, bytepos++)
                if (bufbyte[bytepos] != preamble[bytepos])
                {
                    bytepos = 0;
                    checkPreamble = false;
                    break;
                }

            if (checkPreamble)
            {
                if (bytepos == preamble.Length)
                {
                    CompressBuffer(bytepos);
                    bytepos = 0;
                    checkPreamble = false;
                    detectEncoding = false;
                }

                return true;
            }

            return false;
        }

        protected virtual int ReadBuffer()
        {
            charlen = 0;
            charpos = 0;

            if (!checkPreamble)
                bytelen = 0;

            do
            {
                if (checkPreamble)
                {
                    int len = stream.Read(bufbyte, bytepos, bufbyte.Length - bytepos);

                    if (len == 0)
                    {
                        if (bytelen > 0)
                        {
                            charlen += decoder.GetChars(bufbyte, 0, bytelen, bufchar, charlen);
                            bytepos = 0;
                            bytelen = 0;
                        }

                        return charlen;
                    }

                    bytelen += len;
                }
                else
                {
                    bytelen = stream.Read(bufbyte, 0, bufbyte.Length);

                    if (bytelen == 0)
                        return charlen;
                }

                isBlocked = bytelen < bufbyte.Length;

                if (IsPreamble())
                    continue;

                if (detectEncoding && bytelen >= 2)
                    DetectEncoding();

                charlen += decoder.GetChars(bufbyte, 0, bytelen, bufchar, charlen);
            } while (charlen == 0);

            return charlen;
        }

        public int ReadBuffer(char[] buffer, int offset, int desiredChars, out bool readToUserBuffer)
        {
            charlen = 0;
            charpos = 0;

            if (checkPreamble)
                bytelen = 0;

            int charsRead = 0;

            readToUserBuffer = desiredChars >= bufchar.Length;

            do
            {
                if (checkPreamble)
                {
                    int len = stream.Read(bufbyte, bytepos, bufbyte.Length - bytepos);

                    if (len == 0)
                    {
                        if (bytelen > 0)
                            if (readToUserBuffer)
                            {
                                charsRead += decoder.GetChars(bufbyte, 0, bytelen, buffer, offset + charsRead);
                                charlen = 0;
                            }
                            else
                            {
                                charsRead = decoder.GetChars(bufbyte, 0, bytelen, bufchar, charsRead);
                                charlen += charsRead;
                            }

                        return charsRead;
                    }

                    bytelen += len;
                }
                else
                {
                    bytelen = stream.Read(bufbyte, 0, bufbyte.Length);

                    if (bytelen == 0)
                        return charlen;
                }

                isBlocked = bytelen < bufbyte.Length;

                if (IsPreamble())
                    continue;

                if (detectEncoding && bytelen >= 2)
                {
                    DetectEncoding();
                    readToUserBuffer = desiredChars >= bufchar.Length;
                }

                charpos = 0;
                if (readToUserBuffer)
                {
                    charsRead += decoder.GetChars(bufbyte, 0, bytelen, buffer, offset + charsRead);
                    charlen = 0;
                }
                else
                {
                    charsRead = decoder.GetChars(bufbyte, 0, bytelen, bufchar, charsRead);
                    charlen += charsRead;
                }

                isBlocked &= charsRead < desiredChars;
            } while (charsRead == 0);

            return charsRead;
        }

        public override string ReadLine()
        {
            if (stream == null)
                Error_ReaderClose();

            CheckAsyncTaskInProgress();

            if (charpos == charlen && ReadBuffer() == 0)
                return null;

            StringBuilder sb = null;
            do
            {
                int x = charpos;
                do
                {
                    char c = bufchar[x];
                    if (c == '\r' || c == '\n')
                    {
                        string s;
                        if (sb != null)
                        {
                            sb.Append(bufchar, charpos, x - charpos);
                            s = sb.ToString();
                        }
                        else
                            s = new string(bufchar, charpos, x - charpos);

                        charpos = x + 1;
                        if (c == '\r' && (charpos < charlen || ReadBuffer() > 0) && bufchar[charpos] == '\n')
                            charpos++;

                        return s;
                    }
                    x++;
                } while (x < charlen);

                x = charlen - charpos;
                if (sb == null)
                    sb = new StringBuilder(x + 80);

                sb.Append(bufchar, charpos, x);
            } while (ReadBuffer() > 0);

            return sb.ToString();
        }

        #region unsafe
        protected static unsafe void BlockCopyCOM<TSrc, TDst>(TSrc[] src, int srcpos, TDst[] dst, int dstpos, int cnt)
            where TSrc : unmanaged
            where TDst : unmanaged
        {
            fixed (TSrc* ptr1 = &src[0])
            fixed (TDst* ptr2 = &dst[0])
                BlockCopy((ulong*)(ptr1 + srcpos), (ulong*)(ptr2 + dstpos), cnt);
        }

        protected static unsafe void BlockCopy<T>(T[] src, int srcpos, T[] dst, int dstpos, int cnt) where T : unmanaged
        {
            fixed (T* ptr1 = &src[0])
            fixed (T* ptr2 = &dst[0])
                BlockCopy((ulong*)(ptr1 + srcpos), (ulong*)(ptr2 + dstpos), cnt);
        }

        protected static unsafe void BlockCopy(ulong* upsrc, ulong* updst, int cnt)
        {
            while (cnt >= 8)
            {
                *updst = *upsrc;

                updst += 8;
                upsrc += 8;

                cnt -= 8;
            }

            byte* psrc = (byte*)upsrc;
            byte* pdst = (byte*)updst;
            if (cnt >= 4)
            {
                *(uint*)pdst = *(uint*)psrc;

                pdst += 4;
                psrc += 4;

                cnt -= 4;
            }

            if (cnt >= 2)
            {
                *(ushort*)pdst = *(ushort*)psrc;

                pdst += 2;
                psrc += 2;

                cnt -= 2;
            }

            if (cnt != 0)
                *pdst = *psrc;
        }
        #endregion

        #region throws
        protected void Error_ReaderClose() => throw new ObjectDisposedException($"{nameof(stream)} is closed");
        #endregion
        #endregion

        #region method 2
        public string TrimmedReadLine()
        {
            if (EndOfStream)
                return null;

            string now;
            do
                now = ReadLine().Trim();
            while (EndOfStream == false && now.Length == 0);

            return now;
        }

        public string TrimmedReadLine(ref int count)
        {
            if (EndOfStream)
                return null;

            string now;
            do
            {
                now = ReadLine().Trim();
                count++;
            }
            while (EndOfStream == false && now.Length == 0);

            return now;
        }

        public bool EndOfLine()
        {
            int read = Peek();
            if (read == '\r')
            {
                charpos++;
                if (Peek() == '\n')
                    charpos++;

                return true;
            }
            else if (read == '\n')
            {
                charpos++;
                return true;
            }
            return false;
        }
        #endregion

        #region disposable
        public override void Close() => Dispose(true);

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!leaveOpen && disposing && stream != null)
                    stream.Dispose();

                if (asyncReadTask != null)
                {
                    if (asyncReadTask.IsCompleted == false)
                        asyncReadTask.Wait();

                    asyncReadTask.Dispose();
                }
            }
            finally
            {
                stream = null;
                encoding = null;
                decoder = null;
                bufbyte = null;
                bufchar = null;
                preamble = null;

                charpos = 0;
                charlen = 0;
                bytelen = 0;
                bytepos = 0;

                detectEncoding = false;
                isBlocked = false;
                leaveOpen = false;
                checkPreamble = false;

                asyncReadTask = null;
            }
        }
        #endregion
    }
}
