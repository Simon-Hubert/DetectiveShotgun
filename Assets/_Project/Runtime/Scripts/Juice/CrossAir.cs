using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;

namespace DShotgun
{
    public class CrossAir : MonoBehaviour
    {
        [Serializable]
        private struct CrossAirAssociation {
            public CrossAirType type;
            public Sprite sprite;
        }
        
        public static CrossAir instance { get; private set; }
        
        [SerializeField] private Image _crossAir;
        [SerializeField] private List<CrossAirAssociation> _crossAirSelection;

        private bool _visibleCursor;
        private bool VisibleCursor
        {
            get => _visibleCursor;
            set {
                Cursor.visible = value;
                _visibleCursor = value;
            }
        }
        
        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
        }
        private void Start() {
            ChangeCrossAir(CrossAirType.INGAME_DEFAULT);
        }

        private void Update() {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            _crossAir.rectTransform.position = mousePos;
            if (mousePos.x > Screen.width || mousePos.x < 0 ||
                mousePos.y > Screen.height || mousePos.y < 0)
            {
                Cursor.visible = true;
            }
            else if (!VisibleCursor && Cursor.visible) Cursor.visible = false;
        }

        public void ChangeCrossAir(int a_typeIndex) => ChangeCrossAir((CrossAirType)a_typeIndex);
        public void ChangeCrossAir(CrossAirType a_type) {

            foreach (var cursor in _crossAirSelection) {
                if (cursor.type == a_type && cursor.sprite != null) {
                    VisibleCursor = false;
                    _crossAir.sprite = cursor.sprite;
                    return;
                }
            }

            VisibleCursor = true;
        }
    }
    
    
    [Serializable]
    public enum CrossAirType {
        NONE = -1,
        
        MENU_DEFAULT = 0,
        MENU_SELECTION = 1,
        
        INGAME_DEFAULT = 10,
        INGAME_INTERACTABLE = 11,
        
        DIALOG_DEFAULT = 20,
        DIALOG_SKIP = 21,
        DIALOG_SHUTUP = 22
    }
}
