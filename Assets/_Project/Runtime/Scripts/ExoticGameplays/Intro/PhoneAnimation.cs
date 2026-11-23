using System;
using UnityEngine;

namespace DShotgun
{
    public class PhoneAnimation : MonoBehaviour
    {
        [SerializeField] private float _horizontalScale = 2.5f;
        [SerializeField] private float _frequency = 50f;
        private bool _ringing = false;

        private Vector3 _origin;

        private void Start()
        {
            _origin = transform.position;
        }

        void Update()
        {
            if (!_ringing) return;
            Vector3 pos = transform.position;
            pos.x = _origin.x + Mathf.Cos(_frequency * Time.time) * _horizontalScale;
            transform.position = pos;
        }

        public void StartRing()
        {
            _ringing = true;
        }

        public void StopRing()
        {
            _ringing = false;
        }
    }
}