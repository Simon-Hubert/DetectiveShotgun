using System;
using UnityEngine;

namespace DShotgun
{
    public class ShotImpact : MonoBehaviour
    {
        private InputReader _ir;
        [SerializeField] private ImpactPool _pool;

        private void Start()
        {
            _ir = GetComponent<InputReader>();
            if (_ir != null)
            {
                _ir.OnLeftClickPressed += Shoot;
            }
        }

        private void OnEnable()
        {
            if (!_ir) return;
            _ir.OnLeftClickPressed += Shoot;
        }

        private void OnDisable()
        {
            if (!_ir) return;
            _ir.OnLeftClickPressed -= Shoot;
        }

        private void Shoot()
        {
            GameObject go = _pool.GetShotImpact();
            go.transform.position = Camera.main.ScreenToWorldPoint(InputReader.PointerPosition);
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;
        }
    }
}
