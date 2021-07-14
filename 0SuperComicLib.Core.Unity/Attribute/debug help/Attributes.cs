using System;
using System.Collections.Generic;

namespace SuperComicWorld
{
    /// <summary>
    /// Attribute가 적용된 Reference value가 Null이 아님을 확인했습니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MarkNotNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Attribute가 적용된 ref 전달 value 또는 할당 가능한 필드가 외부에서 읽기 전용임을 알립니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MarkReadOnlyAttribute : Attribute
    {
    }

    /// <summary>
    /// Unity Inspector 를 통해 할당되는 값임을 알립니다.
    /// </summary>
    public sealed class MarkUnityAllocAttribute : FieldBaseAttribute
    {
    }
}