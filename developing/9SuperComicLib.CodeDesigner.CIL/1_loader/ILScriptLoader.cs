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

        public override bool IsReference(HashedString source, HashedString target)
        {
            throw new NotImplementedException();
        }

        public override Type LoadOrGet(string relativePath, ITypeMap typeMap)
        {
            throw new NotImplementedException();
        }

        // protected override CodeGeneratorBase GetCodeGenerator(TypeBuilder tb, CHashSet<HashedString> map, HashedString hs, IExceptionHandler handler, ITypeMap typeMap) =>
        //     new ILCodeGenerator(this, tb, map, hs, handler, typeMap);
    }
}
