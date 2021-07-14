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

        protected override void OnAwake(bool scanAllFields)
        {
            initScale = transform.localScale = new Vector3(minZoom, minZoom, 1f);

            base.OnAwake(scanAllFields);
        }

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
