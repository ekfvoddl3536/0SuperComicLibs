using UnityEngine.EventSystems;

namespace SuperComicWorld.Controller
{
    public abstract class ModelMouseController : ModelClickController, IScrollHandler
    {
        public abstract void OnScroll(PointerEventData arg);
    }
}
