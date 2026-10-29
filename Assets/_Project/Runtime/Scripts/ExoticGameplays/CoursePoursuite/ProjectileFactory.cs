using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class ProjectileFactory : MonoBehaviour
    {
        
        [SerializeField, Foldout("Perspective")] private Vector2 _p0, _p1, _p2, _p3;

        [SerializeField] private float _spawnInterval;
        [SerializeField] private Projectile _projOriginal;

        private void Start()
        {
            _ = ExecuteAsync();
        }

        private async Awaitable ExecuteAsync()
        {
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

        private void SpawnProjectile()
        {
            Projectile proj = Instantiate(_projOriginal);
            float p = Random.value;
            Vector2 from = Vector2.Lerp(_p0, _p2, p);
            Vector2 to = Vector2.Lerp(_p1, _p3, p);
            proj.Init(from, to, 5f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(_p0,0.15f);
            Gizmos.DrawSphere(_p1,0.15f);
            Gizmos.DrawSphere(_p2,0.15f);
            Gizmos.DrawSphere(_p3,0.15f);
            Gizmos.DrawLine(_p0,_p1);
            Gizmos.DrawLine(_p2,_p3);
        }
    }
}
