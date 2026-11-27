using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class Passerby : MonoBehaviour
    {
        private float _duration;
        private AnimationCurve _backToFrontCurve;
        private bool _isInit = false;
        private bool _fromLeft;
        private Perspective _persp;
        
        [SerializeField, MinMaxSlider(0f, 1f)]
        private Vector2 _maxPercentRange;

        private float _maxPercent;
        
        [SerializeField, MinMaxSlider(0.02f, 10f)]
        private Vector2 _scaleRange;
        
        private float _current = 0f;
        
        public void Init(bool fromLeft, Perspective persp) {
            CoursePoursuiteData data = Resources.Load<CoursePoursuiteData>("CoursePoursuiteData");
            _duration = data.BackToFrontDuration;
            _backToFrontCurve = data.BackToFrontCurve;
            _fromLeft = fromLeft;
            _persp = persp;
            _maxPercent = Random.Range(_maxPercentRange.x, _maxPercentRange.y);
            _isInit = true;
            Destroy(gameObject, _duration);
        }

        private void Update() {
            if (!_isInit) return;
            float p = _current / _duration;
            
            float scaleP = _backToFrontCurve.Evaluate(p);
            Vector2 from = _fromLeft ? GetTo(scaleP) : GetFrom(scaleP);
            Vector2 to = _fromLeft ? GetFrom(scaleP) : GetTo(scaleP);

            float posP = p * _maxPercent;
            
            Vector2 pos = Vector2.Lerp(from, to, posP);
            float scale = Mathf.Lerp(_scaleRange.x, _scaleRange.y, scaleP);
            
            transform.position = pos;
            transform.localScale = new Vector3(scale, scale, scale);
            
            _current += Time.deltaTime;
        }

        private Vector2 GetTo(float p) {
            return Vector2.Lerp(_persp.P0, _persp.P1, p);
        }

        private Vector2 GetFrom(float p) { 
            return Vector2.Lerp(_persp.P2, _persp.P3, p);
        }
    }
}
