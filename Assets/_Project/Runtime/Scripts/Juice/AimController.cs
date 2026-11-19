using System;
using System.Threading;
using SMath;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DShotgun
{
    
    [Serializable]
    public class BlurController
    {
        [SerializeField] private VolumeProfile _volumeProfile;
        [SerializeField] private int _targetBlurPasses;
        [SerializeField] private float _targetBlurIntensity;

        private FullScreenBlurVolumeComponent _blur;

        public void ActivateBlur()
        {
            if(_blur == null) _volumeProfile.TryGet(out _blur);
            _blur.intensity.SetValue(new FloatParameter(_blur.intensity.value, true));
            
        }
        
        public void DeactivateBlur()
        {
            if(_blur == null) _volumeProfile.TryGet(out _blur);
            Debug.Log($"BLURRRED : {_blur}");
            _blur.intensity.SetValue(new FloatParameter(_blur.intensity.value, false));
        }
    }
    
    
    [RequireComponent(typeof(InteractableController))]
    public class AimController : MonoBehaviour
    {

        [SerializeField] private PixelPerfectCamera _camera;
        [SerializeField] private float _zoomDuration = 0.75f;
        [SerializeField] private BlurController _blurController = new BlurController();

        private const int defaultPPU = 100;
        private const int zoomedPPU = 150;
        private int _currentPPU = 100;
        private int _targetPPU = 100;

        public bool IsControllingCamera { get; private set; }

        private Vector2 _origin;

        private readonly Vector2 _refSize = new Vector2(19.2f, 10.8f); //Taille de ref de la cam en unit (sert a calculer les bounds)
        private Vector2 CurrentSize => new Vector2(1920f / _currentPPU, 1080f / _currentPPU);
        private Vector2 MaxOffset => (_refSize - CurrentSize)/2f;

        private CancellationTokenSource _cancel;

        private void Start()
        {
            _origin = _camera.transform.position;
            _blurController.DeactivateBlur();
        }

        private void Update()
        {
            if (!IsControllingCamera) {
                
                return;
            }
            float x = Mouse.current.position.ReadValue().x / 1920;
            float y = Mouse.current.position.ReadValue().y / 1080;
            Debug.Log(_origin);

            x = Mathf.Lerp(-MaxOffset.x, MaxOffset.x, x);
            y = Mathf.Lerp(-MaxOffset.y, MaxOffset.y, y);

            Vector2 target = _origin + new Vector2(x, y);
            _camera.transform.position = new Vector3(target.x, target.y, -10);
        }

        private void OnEnable()
        {
            InteractableController controller = GetComponent<InteractableController>();
            controller.OnAimStarted += AimStarted;
            controller.OnAimStopped += AimStopped;
            TransitionSequence.OnCamTargetChanged += SetOrigin;
            GameManager.OnStateChange += OnGameStateChanged;
        }

        private void OnDisable()
        {
            InteractableController controller = GetComponent<InteractableController>();
            controller.OnAimStarted -= AimStarted;
            controller.OnAimStopped -= AimStopped;
            TransitionSequence.OnCamTargetChanged -= SetOrigin;
            GameManager.OnStateChange -= OnGameStateChanged;
        }
        
        void SetOrigin(Vector2 newOrigin)
        {
            Debug.Log("Setted origin");
            _origin = newOrigin;
        }

        private void AimStopped()
        {
            if (GameManager.CurrentState != GameState.Playing) return;
            Enzoom();
        }

        private void Enzoom()
        {
            int currentPPU = _currentPPU;
            _cancel?.Cancel();
            _cancel = new CancellationTokenSource();
            _ = AimStopped(currentPPU,
                CancellationTokenSource.CreateLinkedTokenSource(_cancel.Token, destroyCancellationToken).Token);
        }
        
        private void OnGameStateChanged(GameState newGameState)
        {
            if (newGameState == GameState.Dialogue || newGameState == GameState.Paused) Enzoom();
        }

        private void AimStarted()
        {
            if (GameManager.CurrentState != GameState.Playing) return;
            int currentPPU = _currentPPU;
            _cancel?.Cancel();
            _cancel = new CancellationTokenSource();
            IsControllingCamera = true;
            _ = AimStarted(currentPPU,
                CancellationTokenSource.CreateLinkedTokenSource(_cancel.Token, destroyCancellationToken).Token);
        }

        private async Awaitable AimStarted(int currentPPU, CancellationToken token)
        {
            try {
                float t = 0;
                float duration = (1-Mathf.InverseLerp(defaultPPU, zoomedPPU, currentPPU))*_zoomDuration;
                while (t < duration) {
                    await Awaitable.EndOfFrameAsync(token);
                    t += Time.deltaTime;
                    float p = t / duration;
                    _currentPPU = Mathf.FloorToInt(Mathf.Lerp(currentPPU, zoomedPPU, p));
                    _camera.assetsPPU = _currentPPU;
                }

                _camera.assetsPPU = zoomedPPU;
            }
            catch (Exception e) {
                //Ignored
            }
        }
        
        private async Awaitable AimStopped(int currentPPU, CancellationToken token)
        {
            try {
                float t = 0;
                float duration = Mathf.InverseLerp(defaultPPU, zoomedPPU, currentPPU)*_zoomDuration;
                while (t < duration) {
                    await Awaitable.EndOfFrameAsync(token);
                    t += Time.deltaTime;
                    float p = t / duration;
                    _currentPPU = Mathf.FloorToInt(Mathf.Lerp(currentPPU, defaultPPU, p));
                    _camera.assetsPPU = _currentPPU;
                }

                _camera.assetsPPU = defaultPPU;
            }
            catch (Exception e) {
                //Ignored
            }
            finally {
                IsControllingCamera = false;
                _camera.transform.position = new Vector3(_origin.x, _origin.y, -10);
            }
        }
    }
}
