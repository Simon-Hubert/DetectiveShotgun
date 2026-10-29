using System;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun
{
    public class Projectile : MonoBehaviour
    {
        private Vector2 _from;
        private Vector2 _to;
        private float _duration;

        private float _current = 0f;
        private bool _isInit = false;

        [SerializeField, MinMaxSlider(0.02f, 10f)]
        private Vector2 _scaleRange;
        [SerializeField] private AnimationCurve _scaleCurve;

        public void Init(Vector2 from, Vector2 to, float duration)
        {
            _from = from;
            _to = to;
            _duration = duration;
            transform.position = _from;
            _isInit = true;
            Destroy(gameObject, duration);
        }

        private void Update()
        {
            if (!_isInit) return;
            float p = _current / _duration;
            transform.position = Vector2.Lerp(_from, _to, p) + new Vector2(0, 2f);
            float scaleP = _scaleCurve.Evaluate(p);
            float scale = Mathf.Lerp(_scaleRange.x, _scaleRange.y, scaleP);
            transform.localScale = new Vector3(scale, scale, scale);
            _current += Time.deltaTime;
            
        }
    }
}
