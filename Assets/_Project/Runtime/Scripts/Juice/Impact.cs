using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DShotgun
{
    public class Impact : MonoBehaviour
    {
        [SerializeField] private float _duration;
        private SpriteRenderer _sr;

        private void Awake()
        {
            Destroy(this.gameObject, _duration);
            _sr = GetComponent<SpriteRenderer>();
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
            transform.rotation = rotation;

            StartCoroutine(SpriteOpacityCoroutine());
        }


        private IEnumerator SpriteOpacityCoroutine()
        {
            float t = 0f;
            while (t < _duration)
            {
                t += Time.deltaTime;
                float p = t / _duration;
                Color color = _sr.color;
                color.a = 1 - p;
                _sr.color = color;
                yield return 0;
            }
        }
        
    }
}
