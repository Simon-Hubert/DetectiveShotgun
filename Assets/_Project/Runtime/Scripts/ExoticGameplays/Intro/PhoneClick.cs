using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class PhoneClick : MonoBehaviour
    {
        [SerializeField] private float _force = 100f;
        [SerializeField] private float _gravityScale = 1f;
        [SerializeField] private float _maxSpeed = 50f;
        [SerializeField, MinMaxSlider(50, 300)] private Vector2 _sideForceOffset;
        [SerializeField] private float _maxSideForceRandom;

        private Vector2 _accel;
        private Vector2 _speed;

        private void FixedUpdate()
        {
            _accel = Vector2.down * (9.81f * _gravityScale);
            _speed += _accel * Time.fixedDeltaTime;
            if (_speed.y < 0)
            {
                _speed.y = Mathf.Clamp(_speed.y, -_maxSpeed, 0f);
            }
            transform.position += (Vector3)_speed * Time.fixedDeltaTime;
        }


        public void OnClick()
        {
            float p = (Mathf.InverseLerp(0, 1920, transform.position.x) - 0.5f)*2f;
            float sideForceSet = Mathf.Lerp(_sideForceOffset.x, _sideForceOffset.y, Mathf.Abs(p)) * -Mathf.Sign(p);
            float sideForceRandom = Random.Range(-_maxSideForceRandom, _maxSideForceRandom)*(1-Mathf.Abs(p));
            float sideForce = sideForceSet + sideForceRandom;
            
            _speed = Vector2.up * _force + Vector2.right * sideForce;
        }
    }
}
