using UnityEngine;
using UnityEngine.InputSystem;

namespace DShotgun
{
    public class RaycastCrtToUI : MonoBehaviour
    {
        [SerializeField] private Collider _screenCollider;
        
        private void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
            // Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos).normalized;

            // RaycastHit2D[] hits = new RaycastHit2D[0];
            // if (_screenCollider.Raycast(mouseWorldPos, hits, Mathf.Infinity) > 0) {
            //     Debug.Log($"CRT touche : {hits[0].point}");
            //     hits[0].
            // }

            if (_screenCollider.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity)) {
                Debug.Log($"CRT touche : {hit.point}");
                
                Vector2 screenUV = hit.textureCoord;
            }
        }
    }
}
