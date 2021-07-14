namespace SuperComicWorld
{
    /// <summary>
    /// 우선 검색을 시도하고, 발견되지 않으면 새로 추가합니다
    /// </summary>
    public sealed class GetOrAddCompAttribute : GetCompBaseAttribute
    {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public GetOrAddCompAttribute()
        {
        }

        /// <summary>
        /// 고급 검색 옵션이 있는 생성자
        /// </summary>
        /// <param name="findRootOnly"></param>
        public GetOrAddCompAttribute(bool findRootOnly) : base(findRootOnly)
        {
        }
    }
}