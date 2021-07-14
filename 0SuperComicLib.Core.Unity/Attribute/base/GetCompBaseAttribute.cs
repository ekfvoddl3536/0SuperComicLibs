namespace SuperComicWorld
{
    /// <summary>
    /// Component 검색형 Attribute 기본
    /// </summary>
    public abstract class GetCompBaseAttribute : FieldBaseAttribute
    {
        internal readonly bool findRootOnly;

        internal GetCompBaseAttribute() : this(true) { }

        internal GetCompBaseAttribute(bool findRootOnly) => this.findRootOnly = findRootOnly;
    }
}