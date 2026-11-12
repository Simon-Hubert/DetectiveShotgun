using System;
using SMath;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DShotgun
{
    public class HoverArrows : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private SpriteRenderer _sr;
        [SerializeField] private UnityEvent _onClickedEvent;

        private bool _isHovered = false;

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            Color color = _sr.color;
            color.a = 0;
            _sr.color = color;
        }

        private void Update()
        {
            Color color = _sr.color;
            if (GameManager.CurrentState == GameState.Playing) {
                color.a = MathsUtils.ExpDecay(color.a, _isHovered ? 1f : 0f, 5f, Time.deltaTime);
            }
            else color.a = 0;
            _sr.color = color;
        }

        public void OnPointerEnter(PointerEventData ed)
        {
            Debug.Log("PointerEntered");
            _isHovered = true;
        }

        public void OnPointerExit(PointerEventData ed)
        {
            Debug.Log("PointerExited");
            _isHovered = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (GameManager.CurrentState != GameState.Playing) return;
            _onClickedEvent?.Invoke();
        }
    }
}
