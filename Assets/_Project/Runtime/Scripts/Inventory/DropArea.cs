using UnityEngine;
using UnityEngine.EventSystems;

namespace DShotgun
{
    public class DropArea : MonoBehaviour, IDropHandler
    {
        private RectTransform _rectTransform;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        public void OnDrop(PointerEventData eventData)
        {
            if(eventData.pointerDrag == null) return;
            UIItem uiItem = eventData.pointerDrag.GetComponent<UIItem>();
            if(uiItem == null) return;

            uiItem.LockPosition();
            uiItem.RectTransform.anchoredPosition = _rectTransform.anchoredPosition;
            uiItem.DroppedInShotgun();
        }
    }
}
