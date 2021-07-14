namespace SuperComicWorld
{
    /// <summary>
    /// 필드의 타입으로 Component를 검색하는 Attribute
    /// </summary>
    public sealed class GetCompAttribute : GetCompBaseAttribute
    {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public GetCompAttribute() { }

        /// <summary>
        /// 고급 검색 옵션이 있는 생성자
        /// </summary>
        public GetCompAttribute(bool findRootOnly) : base(findRootOnly) { }
    }
}