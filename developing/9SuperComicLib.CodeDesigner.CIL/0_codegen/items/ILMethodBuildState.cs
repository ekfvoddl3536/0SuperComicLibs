using System.Reflection.Emit;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ILMethodBuildState
    {
        public CILCode[] methodBody;
        // 아직 발견하지 못한 메소드를 호출한 경우, 그 cilcode 위치를 저장
        // 나중에 다시와서 확인하는겸 코드정리할 예정
        public int[] chkMethods;
        // 마찬가지로 이 메소드를 만드는도중 아직 발견하지 못한 필드가 있다면(상수포함)
        // 그 cilcode의 위치를 저장
        public int[] chkFields;
        public MethodBuilder methodBuilder;

        public ILMethodBuildState(MethodBuilder methodBuilder) =>
            this.methodBuilder = methodBuilder;
    }
}
