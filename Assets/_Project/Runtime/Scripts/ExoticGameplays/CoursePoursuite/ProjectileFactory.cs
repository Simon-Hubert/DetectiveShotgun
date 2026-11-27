using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class ProjectileFactory : MonoBehaviour
    {
        [SerializeField] private float _spawnInterval;
        [SerializeField] private Projectile _projOriginal;
        [SerializeField] private Perspective _persp;
        
        private void Start() {
            _ = ExecuteAsync();
        }

        private async Awaitable ExecuteAsync() {
            try
            {
                while (true)
                {
                    await Awaitable.WaitForSecondsAsync(_spawnInterval, destroyCancellationToken);
                    SpawnProjectile();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SpawnProjectile() {
            Projectile proj = Instantiate(_projOriginal);
            float p = Random.value;
            Vector2 from = Vector2.Lerp(_persp.P0, _persp.P2, p);
            Vector2 to = Vector2.Lerp(_persp.P1, _persp.P3, p);
            proj.Init(from, to);
        }
    }
}
