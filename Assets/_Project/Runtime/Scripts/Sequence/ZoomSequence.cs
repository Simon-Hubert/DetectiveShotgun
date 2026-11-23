using System;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DShotgun
{
    public class ZoomSequence : ASequencable
    {
        [SerializeField] private PixelPerfectCamera _camera;
        [SerializeField] private float _duration;
        [SerializeField] private int _targetPPU;

        private static int _currentlyUsing = -1;
        
        protected override async Awaitable OnPlay()
        {
            try {
                float t = 0;
                float currentPPU = _camera.assetsPPU;
                while (t < _duration) {
                    await Awaitable.NextFrameAsync();
                    if (_currentlyUsing != GetInstanceID() && _currentlyUsing > 0) continue;
                    t += Time.deltaTime;
                    float p = t / _duration;
                    _camera.assetsPPU = Mathf.FloorToInt(Mathf.Lerp(currentPPU, _targetPPU, p));
                }

                _camera.assetsPPU = _targetPPU;
                _currentlyUsing = -1;
            }
            catch (Exception e) {
                //Ignored
            }
        }
    }
}
