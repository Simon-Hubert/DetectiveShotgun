using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class PhoneClick : MonoBehaviour
    {
        [SerializeField] private float _force = 100f;
        [SerializeField] private float _gravityScale = 1f;
        [SerializeField] private float _maxSpeed = 50f;
        [SerializeField, MinMaxSlider(50, 300)] private Vector2 _sideForceOffset;
        [SerializeField, MinMaxSlider(0, 100)] private Vector2 _upForceRandomFactor;
        [SerializeField] private float _maxSideForceRandom;

        private Vector2 _accel;
        private Vector2 _speed;

        private bool _firstClick = true;
        private bool _simulated = false;
        private Vector3 _originalPos;

        [SerializeField] private UnityEvent _onFirstClicked;
        [SerializeField] private UnityEvent _onRestart;
        

        private void Start()
        {
            _originalPos = transform.position;
        }

        private void FixedUpdate()
        {
            
            if(transform.position.x < 0) ReStart();
            if(transform.position.x > 1920) ReStart();
            if(transform.position.y < 0) ReStart();
            
            if (!_simulated) return;
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
            CheckFirstClick();
            float p = (Mathf.InverseLerp(0, 1920, transform.position.x) - 0.5f)*2f;
            float randomUpForceFactor = Random.Range(_upForceRandomFactor.x, _upForceRandomFactor.y);
            float sideForceSet = Mathf.Lerp(_sideForceOffset.x, _sideForceOffset.y, Mathf.Abs(p)) * -Mathf.Sign(p);
            float sideForceRandom = Random.Range(-_maxSideForceRandom, _maxSideForceRandom)*(1-Mathf.Abs(p));
            float sideForce = sideForceSet + sideForceRandom;
            _speed = Vector2.up * (_force+randomUpForceFactor)+ Vector2.right * sideForce;
        }

        private void CheckFirstClick()
        {
            if (_firstClick)
            {
                _firstClick = false;
                OnFirstClick();
            }
        }

        private void OnFirstClick()
        {
            _simulated = true;
            _onFirstClicked?.Invoke();
        }

        public void ReStart()
        {
            _firstClick = true;
            _simulated = false;
            transform.position = _originalPos;
            _onRestart?.Invoke();
        }
    }
}
