using SuperComicLib.DataObject;

namespace SuperComicLib.CodeDesigner
{
    [Config("configs/" + nameof(CodeDesigner) + "/" + nameof(ILCodeGenerator) + "/option.datobj")]
    public static class CILCodegenOpt
    {
        // 0 = dynamic size
        // x < 0 = infinity node
        // x > 0 = limit node (+dynamic size)
        public static readonly int InitialSize = 0;
    }
}
