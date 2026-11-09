using System;
using SMath;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DShotgun
{
    [RequireComponent(typeof(InteractableController))]
    public class AimController : MonoBehaviour
    {

        [SerializeField] private PixelPerfectCamera _camera;
        
        private SecondOrderDynamics<float> _zoomDynamics;

        private const int defaultPPU = 100;
        private const int zoomedPPU = 150;
        private int _currentPPU = 100;
        private int _targetPPU = 100;

        private bool _zooming = false;
        
        private void Start()
        {
            InteractableController controller = GetComponent<InteractableController>();
            controller.OnAimStarted += AimStarted;
            controller.OnAimStopped += AimStopped;
            _zoomDynamics = new SecondOrderDynamics<float>(1, 2, 0, 100, new Linear1D());
        }

        private void OnDisable()
        {
            InteractableController controller = GetComponent<InteractableController>();
            controller.OnAimStarted -= AimStarted;
            controller.OnAimStopped -= AimStopped;
        }

        private void AimStopped()
        {
            Debug.Log("Stopped");
            _targetPPU = defaultPPU;
            _zooming = false;
        }

        private void AimStarted()
        {
            Debug.Log("Started");
            _targetPPU = zoomedPPU;
            _zooming = true;
        }

        private void Update()
        {
            if (!_zooming) return;
            _currentPPU = Mathf.FloorToInt(_zoomDynamics.Update(Time.deltaTime, _targetPPU));
            _camera.assetsPPU = _currentPPU;
        }
    }
}
