using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShotAtMouse : MonoBehaviour
{
    [SerializeField] private Image _bulletImpact;
    [SerializeField] private float _fadeDelay;
    private float _currentDelay;
    
    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            _bulletImpact.rectTransform.position = Mouse.current.position.ReadValue();
            _currentDelay = _fadeDelay;
        }

        if (_currentDelay > 0f) {
            _bulletImpact.color = new Color(1f, 1f, 1f, _currentDelay / _fadeDelay);
        }

        _currentDelay -= Time.deltaTime;
    }
}
