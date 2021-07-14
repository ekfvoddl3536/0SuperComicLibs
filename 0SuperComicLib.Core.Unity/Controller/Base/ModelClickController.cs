using UnityEngine.EventSystems;

namespace SuperComicWorld.Controller
{
    public abstract class ModelClickController : SCMonoBehaviour, IPointerClickHandler
    {
        public virtual void OnPointerClick(PointerEventData arg)
        {
            PointerEventData.InputButton button; ;
            if ((button = arg.button) == PointerEventData.InputButton.Left)
                OnMouseLeftClick(arg);
            else if (button == PointerEventData.InputButton.Right)
                OnMouseRightClick(arg);
            else
                OnMouseClick(arg);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData arg) => OnPointerClick(arg);

        /// <summary>
        /// 좌측 및 우측 버튼 이 외 모든 마우스 버튼의 클릭을 받습니다
        /// </summary>
        protected virtual void OnMouseClick(PointerEventData arg)
        {
        }

        /// <summary>
        /// 자주 사용하는 메소드. 우클릭 이벤트를 받습니다
        /// </summary>
        protected virtual void OnMouseRightClick(PointerEventData arg)
        {
        }

        /// <summary>
        /// 자주 사용하는 메소드. 좌클릭 이벤트를 받습니다
        /// </summary>
        protected virtual void OnMouseLeftClick(PointerEventData arg)
        {
        }
    }
}
