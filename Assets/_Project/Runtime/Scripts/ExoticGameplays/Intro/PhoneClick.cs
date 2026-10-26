using System;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun
{
    public class PhoneClick : MonoBehaviour
    {
        [SerializeField] private float _force = 100f;
        [SerializeField] private float _gravityScale = 1f;
        [SerializeField] private float _maxSpeed = 50f;
        [SerializeField, MinMaxSlider(0, 50)] private Vector2 _sideForceOffset;

        private Vector2 _accel;
        private Vector2 _speed;

        private void FixedUpdate()
        {
            _accel = Vector2.down * (9.81f * _gravityScale);
            _speed += _accel * Time.fixedDeltaTime;
            if (_speed.y < 0)
            {
                _speed = Vector2.ClampMagnitude(_speed, _maxSpeed);
            }
            transform.position += (Vector3)_speed * Time.fixedDeltaTime;
        }


        public void OnClick()
        {
            float p = (Mathf.InverseLerp(0, 1920, transform.position.x) - 0.5f)*2f;
            Debug.Log(p);
            p = Mathf.Lerp(_sideForceOffset.x, _sideForceOffset.y, Mathf.Abs(p)) * Mathf.Sign(p);
            _speed = Vector2.up * _force + Vector2.right * p;
        }
    }
}
