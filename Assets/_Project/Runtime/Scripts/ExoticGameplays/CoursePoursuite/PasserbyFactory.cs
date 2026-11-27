using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class PasserbyFactory : MonoBehaviour
    {
        [SerializeField] private Perspective _persp;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private Passerby _passerbyPrefab;

        private void Start() {
            _ = ExecuteAsync();
        }
        
        private async Awaitable ExecuteAsync() {
            try
            {
                while (true)
                {
                    await Awaitable.WaitForSecondsAsync(_spawnInterval, destroyCancellationToken);
                    SpawnPasserby();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SpawnPasserby() {
            Passerby passerby = Instantiate(_passerbyPrefab);
            float rng = Random.value;
            passerby.Init(rng > 0.5f, _persp);
        }
    }
}
