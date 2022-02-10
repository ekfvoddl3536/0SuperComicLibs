using System;
using System.Reflection.Emit;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ILScriptLoader : ScriptLoader
    {
        public ILScriptLoader(Grammar grammar) : base(grammar)
        {
        }

        public ILScriptLoader(Grammar grammar, PreProcessor preprocessor) : base(grammar, preprocessor)
        {
        }

        public ILScriptLoader(Scanner scanner, PreProcessor preprocessor, LALRParser parser, string rootDirectory) : base(scanner, preprocessor, parser, rootDirectory)
        {
        }

        public override object Load(string relativePath, ITypeMap typeMap)
        {
            throw new NotImplementedException();
        }
    }
}
