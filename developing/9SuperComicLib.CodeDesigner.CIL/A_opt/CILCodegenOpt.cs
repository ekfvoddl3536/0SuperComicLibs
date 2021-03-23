using System.Reflection;
using SuperComicLib.DataObject;

namespace SuperComicLib.CodeDesigner
{
    [Config("configs/" + nameof(CodeDesigner) + "/" + nameof(ILCodeGenerator) + "/settings.datobj")]
    public static class CILCodegenOpt
    {
    }
}
