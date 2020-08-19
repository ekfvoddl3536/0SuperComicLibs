using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public delegate void CreateTokenHandler(ref string text, int line, int row, IAddOnlyList<Token> tokens);
}
