using System;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class ScreenShaker : MonoBehaviour
    {
        [SerializeField] private float _duration;
        [SerializeField] private float _intensity;
        [SerializeField] private float _frequency;
        [SerializeField] private bool _useCurve;
        [SerializeField, ShowIf("_useCurve")] private AnimationCurve _curve;

        private CancellationTokenSource _tokenSource;

        private void OnDisable() {
            _tokenSource?.Cancel();
        }

        public void Shake() {
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            CancellationTokenSource linkedToken =
                CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token, destroyCancellationToken);
            _ = ShakeAsync(linkedToken.Token);
        }

        private async Awaitable ShakeAsync(CancellationToken token) {
            Vector3 origin = transform.position;
            try
            {
                float t = _frequency * _duration;
                float dt = 1f / _frequency;
                float intensity = _intensity;
                for (int i = 0; i < t; i++)
                {
                    if (_useCurve) {
                        intensity = _intensity * _curve.Evaluate(1-(i / t));
                    }
                    
                    transform.position = origin + (Vector3)(Random.insideUnitCircle * intensity);
                    await Awaitable.WaitForSecondsAsync(dt, token);
                }
            }
            catch (Exception e)
            {
                //Ignored
            }
            finally
            {
                transform.position = origin;
            }
        }
    }
}
