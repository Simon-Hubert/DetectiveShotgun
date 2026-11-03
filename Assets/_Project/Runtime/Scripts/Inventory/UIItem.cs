using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DShotgun
{
    public class UIItem : MonoBehaviour, IDragHandler
    {
        [SerializeField] private UnityEvent UnityOnSpriteChanged;
        private RectTransform _rectTransform;
        private SpriteRenderer _spriteRenderer;
        private int _id;
        
        public RectTransform RectTransform => _rectTransform;
        public event Action<int> OnDropInShotgun;
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
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
            _spriteRenderer.sprite = sprite;
            UnityOnSpriteChanged?.Invoke();
        }

        public void DroppedInShotgun()
        {
            OnDropInShotgun?.Invoke(_id);
        }
    }
}
