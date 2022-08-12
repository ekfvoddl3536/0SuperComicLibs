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
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperComicWorld.Controller
{
    public class PortalCamera3DController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IScrollHandler
    {
        public Camera viewCamera_start;
        public Camera portalCamera_end;

        public Transform portalCamera_pivot;

        public float cameraZoomSpeed = 1500f;
        public float cameraMoveSpeed = 100f;
        public float cameraRotateSpeed = 50f;
        public float cameraRollSpeed = 1000f;

        public int zoomLv_step = 4;

        protected bool moveCamera_MouseDown;
        protected Vector3 moveCamera_PrevPos;
        protected int zoomLv = 1;

        protected int zoomCnt;

        protected float
            _camMovePw,
            _camRotaPw,
            _camRollPw;

        protected virtual void Awake()
        {
            if (viewCamera_start == false)
                viewCamera_start = Camera.main;

            Debug.Assert(portalCamera_pivot);

            SCGlobalEventSystem.Register((int)SCGlobalEventCode.UPDATE_MOUSE_SPEED, MouseSpeedUpdated_CB);
            OnMouseSpeedUpdated(SCOptions.MouseSensitivity);
        }

        private void MouseSpeedUpdated_CB(object _, object value) => OnMouseSpeedUpdated((float)value);

        protected virtual void OnMouseSpeedUpdated(float value)
        {
            _camMovePw = cameraMoveSpeed * value;

            _camRotaPw = cameraRotateSpeed * value;

            _camRollPw = cameraRollSpeed * value;
        }

        public virtual void OnScroll(PointerEventData arg)
        {
            var tr = portalCamera_end.transform;

            float delta = arg.scrollDelta.y * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                // roll 조정
                tr.Rotate(0, 0, delta * _camRollPw);
            }
            else
            {
                // zoom
                tr.localPosition += tr.forward * (delta * cameraZoomSpeed);

                if (++zoomCnt >= zoomLv_step)
                {
                    zoomCnt = 0;

                    if (arg.scrollDelta.y > 0) // up-scroll
                        zoomLv = Math.Min(zoomLv + 1, 20_000);
                    else
                        zoomLv = Math.Max(zoomLv - 1, 1);
                }
            }
        }

        public virtual void OnPointerUp(PointerEventData arg)
        {
            if (arg.button == PointerEventData.InputButton.Right)
            {
                moveCamera_MouseDown = false;
                moveCamera_PrevPos = default;
            }
        }

        public virtual void OnPointerDown(PointerEventData arg)
        {
            if (arg.button == PointerEventData.InputButton.Right)
            {
                moveCamera_MouseDown = true;
                moveCamera_PrevPos = arg.position;
            }
        }

        protected virtual void Update()
        {
            if (moveCamera_MouseDown)
            {
                Vector3 mouse_pos = Input.mousePosition;

                OnMouseRightButton(mouse_pos, Time.deltaTime);

                moveCamera_PrevPos = mouse_pos;
            }
        }

        protected virtual void OnMouseRightButton(in Vector3 mouse_pos, float dt)
        {
            Vector3 delta = mouse_pos - moveCamera_PrevPos;

            var tr = portalCamera_end.transform;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                delta *= _camRotaPw * dt;

                var tr2 = portalCamera_pivot;

                Vector3 pivot_pos = tr2.position;
                tr.RotateAround(pivot_pos, tr.right, -delta.y);
                tr.RotateAround(pivot_pos, tr.up, delta.x);
            }
            else
            {
                delta *= _camMovePw * (1f / Math.Max(1, zoomLv)) * dt;
                tr.Translate(-tr.LocalRight() * delta.x + -tr.LocalUp() * delta.y);
            }
        }
    }
}
