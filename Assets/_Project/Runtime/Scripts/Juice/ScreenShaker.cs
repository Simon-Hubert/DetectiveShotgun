using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class ScreenShaker : MonoBehaviour
    {
        [SerializeField] private float _duration;
        [SerializeField] private float _intensity;
        [SerializeField] private float _frequency;

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
                for (int i = 0; i < t; i++)
                {
                    transform.position = origin + (Vector3)(Random.insideUnitCircle * _intensity);
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
