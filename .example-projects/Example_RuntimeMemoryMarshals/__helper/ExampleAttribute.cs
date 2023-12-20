using System;

namespace Example_RuntimeMemoryMarshals
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class ExampleAttribute : Attribute
    {
        public readonly string Title;
        public readonly string Summary;

        public ExampleAttribute(string title, string summary)
        {
            Title = title;
            Summary = summary;
        }
    }
}
