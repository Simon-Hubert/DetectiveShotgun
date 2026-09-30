using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [SerializeField] InputActionAsset _inputActionAsset;

    private InputActionMap _actionMap;
    private InputAction _point;
    private InputAction _leftClick;
    private InputAction _rightClick;
    private InputAction _scroll;

    public event Action OnLeftClickPressed;
    public event Action OnRightClickPressed;
    public event Action<int> OnScrolled;
    
    public Vector2 PointerPosition => _point.ReadValue<Vector2>();
    public bool IsLeftHeld => _leftClick.IsPressed();
    public bool IsRightHeld => _rightClick.IsPressed();
    
    private void Awake()
    {
        _actionMap = _inputActionAsset.FindActionMap("Player", throwIfNotFound: true);
        _point = _actionMap.FindAction("Point", true);
        _leftClick = _actionMap.FindAction("LeftClick", true);
        _rightClick = _actionMap.FindAction("RightClick", true);
        _scroll = _actionMap.FindAction("Scroll", true);
    }

    private void OnEnable()
    {
        _leftClick.performed += OnLeftClickPerformed;
        _rightClick.performed += OnRightClickPerformed;
        _scroll.performed += OnScrollPerformed;
        _actionMap.Enable();
    }

    private void OnScrollPerformed(InputAction.CallbackContext obj)
    {
        float y = obj.ReadValue<float>();
        if(Mathf.Approximately(y, 0f)) return;
        OnScrolled?.Invoke(y > 0f ? 1 : -1);
    }

    private void OnRightClickPerformed(InputAction.CallbackContext obj) => OnRightClickPressed?.Invoke();
    
    private void OnLeftClickPerformed(InputAction.CallbackContext obj) => OnLeftClickPressed?.Invoke();

    private void OnDisable()
    {
        _leftClick.performed -= OnLeftClickPerformed;
        _rightClick.performed -= OnRightClickPerformed;
        _scroll.performed -= OnScrollPerformed;
        _actionMap.Disable();
    }
}
