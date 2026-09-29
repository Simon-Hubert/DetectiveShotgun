using UnityEngine;
using UnityEngine.InputSystem;

public class ShotAtMouse : MonoBehaviour
{
    [SerializeField] private RectTransform _bulletImpact;
    
    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            _bulletImpact.position = Mouse.current.position.ReadValue();
        }
    }
}
