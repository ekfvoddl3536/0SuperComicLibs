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

using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperComicWorld.Controller
{
    public class Model3DController : ModelMouseController
    {
        #region fields
        // zoom control
        public float minZoom = 1;
        public float maxZoom = 50;
        public float zoomSpeed = 1;
        private Vector3 initScale;
        #endregion

        protected override void Awake() => initScale = transform.localScale = new Vector3(minZoom, minZoom, 1f);

        public override void OnScroll(PointerEventData arg) =>
            Zoom2D(transform, initScale, zoomSpeed * arg.scrollDelta.y, maxZoom);

        private static void Zoom2D(Transform transform, Vector3 minimumScale, float zoomDelta, float maximumScaleValue)
        {
            Vector3 f_scale =
                Vector3.Min(
                    minimumScale * maximumScaleValue,
                    Vector3.Max(minimumScale, transform.localScale + (Vector3)(Vector2.one * zoomDelta)));

            f_scale.z = 1.0f;

            transform.localScale = f_scale;
        }
    }
}
