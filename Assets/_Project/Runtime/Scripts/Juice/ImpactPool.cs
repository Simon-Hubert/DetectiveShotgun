using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class ImpactPool : MonoBehaviour
    {
        [SerializeField] private GameObject _shotImpactPrefab;
        [SerializeField] private int _maxImpact;

        private readonly Queue<GameObject> _inactiveShotImpactPool = new Queue<GameObject>();
        private readonly Queue<GameObject> _activeShotImpactPool = new Queue<GameObject>();

        public GameObject GetShotImpact()
        {
            if (_inactiveShotImpactPool.Count > 0) {
                GameObject go = _inactiveShotImpactPool.Dequeue();
                _activeShotImpactPool.Enqueue(go);
                go.SetActive(true);
                return go;
            }

            if (_activeShotImpactPool.Count > 0) {
                GameObject go2 = _activeShotImpactPool.Dequeue();
                _activeShotImpactPool.Enqueue(go2);
                return go2;
            }

            throw new ArgumentException("All Pool's Queues are Empty");
        }

        private void Start()
        {
            for (int i = 0; i < _maxImpact; i++) {
                GameObject go = Instantiate(_shotImpactPrefab, transform, true);
                go.SetActive(false);
                go.transform.rotation = Quaternion.Euler(0,0,Random.Range(0f,360f));
                _inactiveShotImpactPool.Enqueue(go);
            }
        }
    }
}
