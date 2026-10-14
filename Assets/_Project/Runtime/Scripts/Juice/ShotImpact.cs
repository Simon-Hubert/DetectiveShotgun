using System;
using UnityEngine;

namespace DShotgun
{
    public class ShotImpact : MonoBehaviour
    {
        private InputReader _ir;
        [SerializeField] private GameObject _shotImpactPrefab;

        private void Start()
        {
            _ir = GetComponent<InputReader>();
            _ir.OnLeftClickPressed += Shoot;
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
            Debug.Log("Shooted the gun");
            GameObject go = Instantiate(_shotImpactPrefab);
            go.transform.position = Camera.main.ScreenToWorldPoint(_ir.PointerPosition);
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;
        }
    }
}
