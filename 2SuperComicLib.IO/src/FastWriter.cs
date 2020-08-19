using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Text
{
    public unsafe class FastWriter : IDisposable
    {
        protected Stream ss;

        protected FastWriter() { }

        public FastWriter(Stream baseStream) => ss = baseStream;

        public FastWriter(string filepath) => ss = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        public FastWriter(string filepath, FileMode mode, FileAccess access) => ss = File.Open(filepath, mode, access);

        #region 메서드
        protected void WriteUnsafe(byte* ptr, int length)
        {
            // for (int x = 0; x < length; x++)
            //     ss.WriteByte(ptr[x]);
        }

        public virtual void Write_c<T>(T value) where T : unmanaged => WriteUnsafe((byte*)&value, sizeof(T));

        public virtual void Write<T>(ref T rvalue) where T : unmanaged
        {
            fixed (T* ptr = &rvalue)
                WriteUnsafe((byte*)ptr, sizeof(T));
        }

        public virtual void Write(byte[] data) => ss.Write(data, 0, data.Length);

        public virtual void Write(byte data) => ss.WriteByte(data);

        public virtual void Write(sbyte data) => ss.WriteByte(*(byte*)&data);

        public virtual void Write(bool data) => ss.WriteByte(data ? (byte)1 : byte.MinValue);

        public virtual void Write(char data) => WriteUnsafe((byte*)&data, sizeof(char));

        public virtual void Write(short data) => WriteUnsafe((byte*)&data, sizeof(short));

        public virtual void Write(ushort data) => WriteUnsafe((byte*)&data, sizeof(ushort));

        public virtual void Write(int data) => WriteUnsafe((byte*)&data, sizeof(int));

        public virtual void Write(uint data) => WriteUnsafe((byte*)&data, sizeof(uint));

        public virtual void Write(long data) => WriteUnsafe((byte*)&data, sizeof(long));

        public virtual void Write(ulong data) => WriteUnsafe((byte*)&data, sizeof(ulong));

        public virtual void Write(float data) => WriteUnsafe((byte*)&data, sizeof(float));

        public virtual void Write(double data) => WriteUnsafe((byte*)&data, sizeof(double));

        public virtual void Write(char[] data)
        {
            fixed (char* ptr = data)
                WriteUnsafe((byte*)ptr, data.Length * sizeof(char));
        }

        public virtual void Write(string strdata)
        {
            if (string.IsNullOrWhiteSpace(strdata))
                Write(0);
            else
            {
                // char[] arr = strdata.ToCharArray();
                // Write(arr.Length);
                // Write(arr);
                fixed (char* ptr = strdata)
                {
                    int length = strdata.Length;
                    Write(length);
                    WriteUnsafe((byte*)ptr, length * sizeof(char));
                }
            }
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
        ~FastWriter()
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
