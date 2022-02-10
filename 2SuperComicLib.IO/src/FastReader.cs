using System;
using System.IO;

namespace SuperComicLib.IO
{
    public unsafe class FastReader : IDisposable
    {
        protected Stream ss;

        protected FastReader() { }

        public FastReader(Stream baseStream) => ss = baseStream;

        public FastReader(string filepath) => ss = File.Open(filepath, FileMode.Open, FileAccess.Read);

        #region 메서드
        protected T ReadUnsafe<T>(int length) where T : unmanaged
        {
            byte[] temp = new byte[length];
            ss.Read(temp, 0, length);
            fixed (byte* ptr = temp)
                return *(T*)ptr;
        }

        public virtual T ReadTo<T>() where T : unmanaged => ReadUnsafe<T>(sizeof(T));

        public virtual bool ReadBoolean() => ss.ReadByte() > 0;

        public virtual byte ReadUInt8() => (byte)ss.ReadByte();

        public virtual sbyte ReadInt8() => (sbyte)ss.ReadByte();

        public virtual char ReadChar() => ReadUnsafe<char>(sizeof(char));

        public virtual short ReadInt16() => ReadUnsafe<short>(sizeof(short));

        public virtual ushort ReadUInt16() => ReadUnsafe<ushort>(sizeof(ushort));

        public virtual int ReadInt32() => ReadUnsafe<int>(sizeof(int));

        public virtual uint ReadUInt32() => ReadUnsafe<uint>(sizeof(uint));

        public virtual float ReadSingle() => ReadUnsafe<float>(sizeof(float));

        public virtual long ReadInt64() => ReadUnsafe<long>(sizeof(long));

        public virtual ulong ReadUInt64() => ReadUnsafe<ulong>(sizeof(ulong));

        public virtual double ReadDouble() => ReadUnsafe<double>(sizeof(double));

        public virtual byte[] ReadBytes(int length)
        {
            byte[] temp = new byte[length];
            ss.Read(temp, 0, length);
            return temp;
        }

        public virtual char[] ReadChars(int length)
        {
            char[] result = new char[length];
            fixed (byte* ptr = ReadBytes(length * 2))
            fixed (char* dst = result)
            {
                char* src = (char*)ptr;
                for (int x = 0; x < length; x++)
                    dst[x] = src[x];
            }
            return result;
        }

        public virtual string ReadString()
        {
            int length = ReadInt32();
            return length == 0 ? string.Empty : new string(ReadChars(length));
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ss.Close();
                ss = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~FastReader()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(false);
        }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
