using System;
using System.Linq;
using SMath;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace DShotgun
{
    /// <summary>
    /// Écoute l'<see cref="InputReader"/> et ne décide rien lui-même : les conditions et les priorités
    /// sont gérées par le manager de l'objet touché.
    /// Détecte l'objet interactif visé par le joueur et déclenche son <see cref="InteractableManager"/>.
    /// </summary>
    public class InteractableController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;
        [SerializeField] private InputReader _inputs;

        [Header("Aim Params")]
        [SerializeField] private bool _toggleAimOnRightClick = false;

        [Header("Aim Zoom")]
        [SerializeField, Min(1)] private int _defaultPpu = 100;
        [SerializeField, Min(1)] private int _aimPpu = 150;
        [SerializeField, Min(0.01f)] private float _zoomDuration = 0.3f;
        [SerializeField] private AnimationCurve _zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Aim Follow")]
        [SerializeField] private Vector2 _aimMaxOffset = new Vector2(3f, 2f);
        [SerializeField, Min(0.01f)] private float _frequency = 2f;
        [SerializeField, Min(0f)] private float _damping = 1f;
        [SerializeField] private float _response = 0f;

        private bool _isAiming;
        private bool _isPointerOverUI;
        private bool _isBusy;
        private InteractableManager[] _aimed = Array.Empty<InteractableManager>();
        private Vector3 _defaultPosition;
        public Vector3 SetDefaultCamPos { set => _defaultPosition = value; }
        private float _zoomProgress;
        private SecondOrderDynamics<Vector2> _moveDynamics;

        private void Start()
        {
            if (_camera == null) _camera = Camera.main;
            if (_pixelPerfectCamera == null) _pixelPerfectCamera = _camera.GetComponent<PixelPerfectCamera>();

            _defaultPosition = _camera.transform.position;
            CreateDynamics();
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && _moveDynamics != null) CreateDynamics();
        }
#endif

        private void Update()
        {
            _isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!_toggleAimOnRightClick) _isAiming = _inputs.IsRightHeld;

            InteractableManager[] targets = _isAiming ? GetInteractableUnderPointer() : Array.Empty<InteractableManager>();
            
            if (!_isBusy)
            {
                foreach (InteractableManager target in targets)
                {
                    if (!_aimed.Contains(target) && target.Aim()) break;
                }
            }
            _aimed = targets;
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f) return;

            _zoomProgress = Mathf.MoveTowards(_zoomProgress, _isAiming ? 1f : 0f, deltaTime / _zoomDuration);
            float zoom = _zoomCurve.Evaluate(_zoomProgress);
            _pixelPerfectCamera.assetsPPU = Mathf.Max(1, Mathf.RoundToInt(Mathf.LerpUnclamped(_defaultPpu, _aimPpu, zoom)));

            Vector2 targetPosition = (Vector2)_defaultPosition + (_isAiming ? GetCursorOffset() : Vector2.zero);
            Vector2 position = _moveDynamics.Update(deltaTime, targetPosition);
            _camera.transform.position = new Vector3(position.x, position.y, _defaultPosition.z);
        }

        private void HandleScrolled(int obj)
        {
            throw new System.NotImplementedException();
        }

        private void HandleRightClickPressed()
        {
            if (_toggleAimOnRightClick) _isAiming = !_isAiming;
        }

        private void HandleLeftClickPressed()
        {
            if (_isBusy) return;

            foreach (InteractableManager interactable in GetInteractableUnderPointer())
            {
                if (interactable.Interact()) break;
            }
        }

        private InteractableManager[] GetInteractableUnderPointer()
        {
            if (_isPointerOverUI) return Array.Empty<InteractableManager>();

            Vector2 worldPoint = _camera.ScreenToWorldPoint(_inputs.PointerPosition);
            return Physics2D.OverlapPointAll(worldPoint)
                .Select(hit => hit.GetComponent<InteractableManager>())
                .Where(interactable => interactable != null)
                .OrderByDescending(interactable => interactable.Priority)
                .ToArray();
        }

        private void CreateDynamics()
        {
            _moveDynamics = new SecondOrderDynamics<Vector2>(_frequency, _damping, _response, _camera.transform.position, new Linear2D());
        }

        private Vector2 GetCursorOffset()
        {
            Vector2 viewport = _camera.ScreenToViewportPoint(_inputs.PointerPosition);
            Vector2 centered = new Vector2(Mathf.Clamp(viewport.x * 2f - 1f, -1f, 1f), Mathf.Clamp(viewport.y * 2f - 1f, -1f, 1f));
            return Vector2.Scale(centered, _aimMaxOffset);
        }
    }
}
