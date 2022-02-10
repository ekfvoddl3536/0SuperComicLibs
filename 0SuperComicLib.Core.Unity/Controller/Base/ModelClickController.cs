#region LICENSE
/* MIT License
 * 
 * Copyright (c) 2021 SuperComic <ekfvoddl3535@naver.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE
 */
#endregion

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
