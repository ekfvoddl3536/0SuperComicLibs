using System;
using System.Runtime.CompilerServices;

namespace ExampleProject
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class ExampleAttribute : Attribute
    {
        public readonly string Title;
        public readonly string Summary;

        public readonly string File;

        public ExampleAttribute(string title, string summary,
            [CallerFilePath] string file = "")
        {
            Title = title;
            Summary = summary;

            File = file;
        }
    }
}
