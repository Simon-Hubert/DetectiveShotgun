using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DShotgun
{
    public class CanPile : MonoBehaviour
    {
        [SerializeField] private float _force;
        private Rigidbody2D[] _cans;
        
        private void Awake()
        {
            List<Rigidbody2D> cans = new List<Rigidbody2D>();
            foreach (Transform child in transform) {
                if (!child.gameObject.activeSelf) continue;
                Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
                if(rb) cans.Add(rb);
            }
            _cans = cans.ToArray();
        }
        
        public void OnShoot()
        {
            foreach (Rigidbody2D can in _cans)
            {
                Vector2 dir = can.transform.position - transform.position;
                dir.Normalize();
                can.simulated = true;
                can.AddForceAtPosition(dir * _force, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
                Destroy(can.gameObject, 5f);
            }
        }
    }
}
