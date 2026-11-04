using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DShotgun
{
    public class UIItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private UnityEvent UnityOnSpriteChanged;
        private RectTransform _rectTransform;
        private Image _img;
        private CanvasGroup _canvasGroup;
        
        private int _id;
        private bool _isLocked;
        private Vector3 _startDragPos;
        
        public RectTransform RectTransform => _rectTransform;
        public event Action<int> OnDropInShotgun;
        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _img = GetComponent<Image>();
        }
        
        public void SetId(int id)
        {
            _id = id;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta;
        }

        public void ChangeSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }
            else
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
            }
            _img.sprite = sprite;
            UnityOnSpriteChanged?.Invoke();
        }
        
        public void LockPosition() => _isLocked = true;
        public void UnlockPosition() => _isLocked = false;

        public void DroppedInShotgun()
        {
            OnDropInShotgun?.Invoke(_id);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;
            _startDragPos = _rectTransform.anchoredPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            if (_isLocked) return;
            _rectTransform.anchoredPosition = _startDragPos;
        }

        public void ResetPosition()
        {
            _rectTransform.anchoredPosition = _startDragPos;
        }
    }
}
