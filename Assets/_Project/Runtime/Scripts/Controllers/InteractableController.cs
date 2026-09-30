using System;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private InputReader _inputs;

    private void Start()
    {
        if(_camera == null) _camera = Camera.main;
    }

    private void OnEnable()
    {
        _inputs.OnLeftClickPressed += HandleLeftClickPressed;
        _inputs.OnRightClickPressed += HandleRightClickPressed;
        _inputs.OnScrolled += HandleScrolled;
    }

    private void HandleScrolled(int obj)
    {
        throw new System.NotImplementedException();
    }

    private void HandleRightClickPressed()
    {
        throw new System.NotImplementedException();
    }

    private void HandleLeftClickPressed()
    {
        Vector2 worldPoint = _camera.ScreenToWorldPoint(_inputs.PointerPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);
        if(hit != null && hit.TryGetComponent(out InteractableManager interactable))
        {
            if (interactable.Interact())
            {
                
                Debug.Log($"Interacted with {interactable.name}");
            }
        }
    }
}
