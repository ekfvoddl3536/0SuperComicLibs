using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public abstract class ScriptLoader : IDisposable
    {
        internal Scanner m_scanner;
        internal PreProcessor m_preprocessor;
        internal LALRParser m_parser;
        internal string path;
        internal AssemblyBuilder asmbd;
        internal ModuleBuilder modbd;
        internal Dictionary<HashedString, Type> items;
        internal Dictionary<HashedString, CHashSet<HashedString>> references;
        internal CHashSet<HashedString> cantLoad;

        #region constructor
        private ScriptLoader()
        {
            asmbd =
                AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("9SuperComicLib.CodeDesigner.LoadedScripts"),
                    AssemblyBuilderAccess.RunAndCollect);
            modbd = asmbd.DefineDynamicModule("LoadedScriptsModule");
            
            items = new Dictionary<HashedString, Type>();
            cantLoad = new CHashSet<HashedString>();
            references = new Dictionary<HashedString, CHashSet<HashedString>>();
        }

        public ScriptLoader(Grammar grammar) : 
            this(new Scanner(),
                null, 
                new LALRParser(grammar), 
                CurrentDirectory)
        {
        }

        public ScriptLoader(Grammar grammar, PreProcessor preprocessor) :
            this(new Scanner(),
                preprocessor,
                new LALRParser(grammar),
                CurrentDirectory)
        {
        }
    
        /// <exception cref="ArgumentNullException">rootDirectory is null or empty or whitespace</exception>
        public ScriptLoader(Scanner scanner, PreProcessor preprocessor, LALRParser parser, string rootDirectory) : this()
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ArgumentNullException(nameof(rootDirectory));

            path = rootDirectory;
            m_scanner = scanner;
            m_preprocessor = preprocessor;
            m_parser = parser;
        }
        #endregion

        public bool IsReference(HashedString source, HashedString target) =>
            references.TryGetValue(source, out CHashSet<HashedString> v) &&
            v.Contains(target);

        public bool IsEndlessRef(HashedString v1, HashedString v2) =>
            IsReference(v1, v2) &&
            IsReference(v2, v1);

        public Type LoadOrGet(string relativePath) =>
            LoadOrGet(
                relativePath, 
                ExceptionHandlerFactory.Default,
                TypeTable.Current);

        public Type LoadOrGet(string relativePath, IExceptionHandler handler, ITypeMap typeMap)
        {
            HashedString hs = new HashedString(relativePath);
            if (cantLoad.Contains(hs))
                return null;
            
            if (items.TryGetValue(hs, out Type result))
                return result;

            string absolutePath = Path.Combine(path, relativePath);
            if (File.Exists(absolutePath) == false)
                throw new FileNotFoundException(absolutePath);

            ITokenEnumerator toks =
                m_preprocessor == null
                ? m_scanner.FromFile(absolutePath, Encoding.UTF8)
                : m_scanner.FromStream(m_preprocessor, File.OpenRead(absolutePath), Encoding.UTF8, false);

            if (!references.TryGetValue(hs, out CHashSet<HashedString> map))
            {
                map = new CHashSet<HashedString>();
                references.Add(hs, map);
            }

            TypeBuilder tb = modbd.DefineType(PathToName(relativePath));
            CodeGeneratorBase codegen = GetCodeGenerator(tb, map, hs, handler, typeMap);
            codegen.Generate(m_parser.Parse(toks, true));
            if (handler.FailCount > 0)
            {
                result = null;
                cantLoad.Add(hs);
            }
            else
            {
                result = tb.CreateType();
                items.Add(hs, result);
            }

            codegen.Dispose();
            
            return result;
        }

        protected abstract CodeGeneratorBase GetCodeGenerator(TypeBuilder tb, CHashSet<HashedString> map, HashedString hs, IExceptionHandler handler, ITypeMap typeMap);

        #region path help
        private static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static string PathToName(string relpath)
        {
            int count = relpath.Length;
            StringBuilder strb = new StringBuilder(count + 1);
            strb.Append("d"); // 이름오류 방지

            for (int x = 0; x < count; x++)
                if (char.IsLetterOrDigit(relpath[x]))
                    strb.Append(relpath[x]);
                else
                    strb.Append('_'); // 이름 exception 방지

            return strb.ToString();
        }
        #endregion

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_preprocessor != null)
            {
                m_preprocessor.Dispose();
                m_preprocessor = null;
            }

            if (path != null)
            {
                m_scanner.Dispose();
                m_scanner = null;

                m_parser.Dispose();
                m_parser = null;

                items.Clear();
                items = null;

                references.Clear();
                references = null;

                cantLoad.Dispose();
                cantLoad = null;

                modbd = null;
                asmbd = null;
                path = null;
            }
        }
        #endregion
    }
}
