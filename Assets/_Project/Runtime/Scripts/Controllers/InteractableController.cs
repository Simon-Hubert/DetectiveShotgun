using System;
using System.Collections;
using UnityEngine;

namespace DShotgun
{
    /// <summary>
    /// /// Écoute l'<see cref="InputReader"/> et ne décide rien lui-même : les conditions et les priorités
    /// sont gérées par le manager de l'objet touché.
    /// Détecte l'objet interactif visé par le joueur et déclenche son <see cref="InteractableManager"/>.
    /// </summary>
    public class InteractableController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private InputReader _inputs;
        
        [Header("Aim Params")]
        [SerializeField] private float _aimAnimTime = 1f;
        [SerializeField] private float _defaultFov = 10f;
        [SerializeField] private float _aimFov = 15f;
        [SerializeField] private bool _toggleAimOnRightClick = false;

        private Collider2D _aimHit;
        private bool _cameraZoomed;
        private Coroutine _zoomRoutine;

        private void Start()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void OnEnable()
        {
            _inputs.OnLeftClickPressed += HandleLeftClickPressed;
            _inputs.OnRightClickPressed += HandleRightClickPressed;
            _inputs.OnScrolled += HandleScrolled;
        }

        private void OnDisable()
        {
            _inputs.OnLeftClickPressed -= HandleLeftClickPressed;
            _inputs.OnRightClickPressed -= HandleRightClickPressed;
            _inputs.OnScrolled -= HandleScrolled;
        }

        private void Update()
        {
            if (!_toggleAimOnRightClick)
            {
                if (_inputs.IsRightHeld)
                {
                    HandleRightClickHeld();
                }
                else
                {
                    HandleRightClickReleased();
                }
       
            }
        }

        private void HandleScrolled(int obj)
        {
            throw new System.NotImplementedException();
        }

        private void HandleRightClickPressed()
        {
          
        }
        private void HandleRightClickHeld()
        {
            if (!_cameraZoomed) ZoomCamera();
            Vector2 worldPoint = _camera.ScreenToWorldPoint(_inputs.PointerPosition);
            _aimHit = Physics2D.OverlapPoint(worldPoint);
            if (_aimHit != null && _aimHit.TryGetComponent(out InteractableManager interactable))
            {
                interactable.OnAimed();
            }
        }
        
        private void HandleRightClickReleased()
        {
            if(_cameraZoomed)
            {
                ResetCamera();
            }
        }

        private void HandleLeftClickPressed()
        {
            Vector2 worldPoint = _camera.ScreenToWorldPoint(_inputs.PointerPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPoint);
            if (hit != null && hit.TryGetComponent(out InteractableManager interactable))
            {
                if (interactable.Interact())
                {

                    Debug.Log($"Interacted with {interactable.name}");
                }
            }
        }
        
        private void ZoomCamera()
        {
            _cameraZoomed = true;
            _zoomRoutine = StartCoroutine(CameraZoomRoutine(true));
        }
        
        private IEnumerator CameraZoomRoutine(bool zoom)
        {
            float elapsedTime = 0.0f;
            while(elapsedTime < _aimAnimTime)
            {
                if(zoom)_camera.orthographicSize = Mathf.Lerp(_defaultFov, _aimFov, elapsedTime / _aimAnimTime);
                else _camera.orthographicSize = Mathf.Lerp(_aimFov, _defaultFov, elapsedTime / _aimAnimTime);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        private void ResetCamera()
        {
            _cameraZoomed = false;
            _zoomRoutine = StartCoroutine(CameraZoomRoutine(false));
        }
        
    }
}
