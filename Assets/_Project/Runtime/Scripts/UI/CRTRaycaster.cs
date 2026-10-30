using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DShotgun
{
    public class CRTRaycaster : BaseRaycaster
    {
        [Header("On Scene")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Collider _screenCollider;
        [SerializeField] private GraphicRaycaster _uiRaycaster;
        
        [Header("texture Parameters")]
        [SerializeField] private RenderTexture _uiTexture;
        [Space(5)]
        [SerializeField,Range(0f, 1f)] private float _textureDistortion = 0.06f;
        [SerializeField,Range(1f, 1.5f)] private float _textureZoom = 1.04f;
        
        public override Camera eventCamera => _camera;
        
        
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
            if (Camera.main == null) return;
            
            // Raycast from mianCamera
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

            if (_screenCollider.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity)) {
                Vector2 screenUV = hit.textureCoord;
                
                // transformation from UV screen to UV Shader
                //
                // Pcrt = (Porigine * (1 + k * r^2)) / z
                // Pcrt = Projection CRT | Porgine = Projection origin (Camera)
                // k = Distortion | r = distortion radius | z = Zoom
                
                Vector2 p = (screenUV - Vector2.one * 0.5f) * 2f; // Porigin
                float rc = Vector2.Dot(p, p); // r^2

                Vector2 sourceUV = Vector2.one * 0.5f + p * (0.5f * (1f + _textureDistortion * rc)) / _textureZoom; // Pcrt
                //Debug.Log($"CRT touche : ScreenUV = {screenUV} | SourceUV = {sourceUV}");
                
                if (sourceUV.x < 0f || sourceUV.x > 1f ||
                    sourceUV.y < 0f || sourceUV.y > 1f)
                    return;

                // Canvas correction
                Vector2 correctedPos = new Vector2(
                    sourceUV.x * _uiTexture.width, 
                    sourceUV.y * _uiTexture.height);
    
                // Canvas Raycast
                PointerEventData correctedUIEvent = new PointerEventData(EventSystem.current);
                
                correctedUIEvent.position = correctedPos;
                correctedUIEvent.button = eventData.button;
                correctedUIEvent.pointerId = eventData.pointerId;
                
                _uiRaycaster.Raycast(correctedUIEvent, resultAppendList);
            }
        }
    }
}
