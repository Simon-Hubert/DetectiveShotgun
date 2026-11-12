using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DShotgun
{
    public class UIHover : MonoBehaviour, IPointerEnterHandler
    {
        private Image _im;
        private RectTransform _rt;
        private CanvasGroup _cg;

        private bool _isHovered = false;

        private void Start()
        {
            _im = GetComponent<Image>();
            _cg = GetComponent<CanvasGroup>();
            _rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (GameManager.CurrentState == GameState.Playing) {
                _cg.alpha = MathsUtils.ExpDecay(_cg.alpha, _isHovered ? 0f : 1f, 5f, Time.deltaTime);
            }
            else _cg.alpha = 1;
            
            if (!_isHovered) return;
            
            bool stillInside = RectTransformUtility.RectangleContainsScreenPoint(
                _rt, 
                Mouse.current.position.ReadValue(), 
                null 
            );

            if (!stillInside)
            {
                OnPointerExit();
            }
        }

        public void OnPointerEnter(PointerEventData ed)
        {
            _isHovered = true;
            _cg.blocksRaycasts = false;
        }

        private void OnPointerExit()
        {
            _isHovered = false;
            _cg.blocksRaycasts = true;
        }
        
        

        
    }
}
