using System;

namespace SuperComicLib.LowLevel
{
    public unsafe abstract class PointerMethods<T> : IDisposable
    {
        protected PointerMethods() { }

        public abstract T Default(void* ptr);

        public virtual IntPtr GetAddr(ref T obj) => NativeClass.GetAddr(ref obj);

        public virtual void GetPinnedPtr(ref T obj, Action<IntPtr> cb) => NativeClass.PinnedAddr(ref obj, cb);

        #region IDisposable Support
        protected abstract void Dispose(bool disposing);

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~PointerMethods()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(false);
        }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
