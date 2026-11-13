using System;
using UnityEngine;
using UnityEngine.UI;

namespace DShotgun
{
    public class PlaceholderUI : MonoBehaviour
    {
        [SerializeField] private Image _im;

        private void Start()
        {
            _im = GetComponent<Image>();
            _im.enabled = false;
        }

        private void OnEnable()
        {
            UIItem.OnUIItemBeginDrag += Show;
            UIItem.OnUIItemEndDrag += Hide;
        }
        
        private void OnDisable()
        {
            UIItem.OnUIItemBeginDrag -= Show;
            UIItem.OnUIItemEndDrag -= Hide;
        }

        private void Hide()
        {
            _im.enabled = false;
        }

        private void Show()
        {
            _im.enabled = true;
        }
    }
}
