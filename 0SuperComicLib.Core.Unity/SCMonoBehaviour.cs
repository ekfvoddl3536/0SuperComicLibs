using UnityEngine;

namespace SuperComicWorld
{
    /// <summary>
    /// 슈퍼코믹이 개조한 MonoBehaviour
    /// </summary>
    public abstract class SCMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 유니티 스크립트가 처음 시작될 때 (최초 1번)
        /// </summary>
        protected void Awake() =>
            OnAwake(true);

        /// <summary>
        /// 프레임이 시작되기 전 ((매번) 최초 Update 호출 전 1번)
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// 게임 오브젝트 삭제될 때
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// Active 가 false 될 때
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// 프레임 마다
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// 프레임 마다 (마지막)
        /// </summary>
        protected virtual void LateUpdate()
        {
        }

        /// <summary>
        /// 초기화
        /// </summary>
        protected virtual void Reset()
        {
        }

        /// <summary>
        /// OnAwake
        /// </summary>
        protected virtual void OnAwake(bool scanAllFields)
        {
            if (scanAllFields)
                this.UpdateAnyFields();
        }
    }
}
