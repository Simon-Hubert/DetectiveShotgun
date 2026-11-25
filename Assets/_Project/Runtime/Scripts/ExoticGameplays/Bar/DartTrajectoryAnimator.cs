using System;
using UnityEngine;

namespace DShotgun
{
    public class DartTrajectoryAnimator : MonoBehaviour
    {
        [Header("Flight")]
        [SerializeField] private Transform _startPoint;
        [SerializeField, Min(0.01f)] private float _flightDuration = 0.75f;
        [SerializeField] private AnimationCurve _flightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Arc (Y)")]
        [SerializeField] private float _arcHeight = 1.2f;

        [Header("Distance (Scale)")]
        [SerializeField, Range(0.05f, 1f)] private float _endScale = 0.35f;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Vector3 _restPosition;
        private Vector3 _restScale;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _elapsed;
        private bool _isFlying;

        public event Action OnEndAnim;

        private void Awake()
        {
            _restPosition = transform.position;
            _restScale = transform.localScale;
        }

        public void Animate(Vector2 hitPoint)
        {
            Vector3 start = _restPosition;
            if (_startPoint != null)
            {
                start = _startPoint.position;
            }
            _startPosition = new Vector3(start.x, start.y, _restPosition.z);

            transform.position = _startPosition;
            transform.localScale = _restScale;

            _targetPosition = new Vector3(hitPoint.x, hitPoint.y, _restPosition.z);
            _elapsed = 0f;
            _isFlying = true;
        }

        private void Update()
        {
            if (!_isFlying) return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _flightDuration);

            Vector3 position = Vector3.LerpUnclamped(_startPosition, _targetPosition, _flightCurve.Evaluate(t));
            position.y += 4f * t * (1f - t) * _arcHeight;
            transform.position = position;

            transform.localScale = _restScale * Mathf.LerpUnclamped(1f, _endScale, _scaleCurve.Evaluate(t));

            if (t >= 1f)
            {
                _isFlying = false;
                OnEndAnim?.Invoke();
            }
        }
    }
}
