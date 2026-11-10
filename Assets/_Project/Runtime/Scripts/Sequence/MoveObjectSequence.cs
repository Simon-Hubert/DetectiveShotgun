using System;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace DShotgun
{
    public class MoveObjectSequence : ASequencable
    {
        [SerializeField] private GameObject _object;
        [SerializeField] private float _duration;
        [SerializeField] private Vector2 _targetPos;

        private float _savedZ;

        private void Start()
        {
            _savedZ = _object.transform.position.z;
        }

        protected override async Awaitable OnPlay()
        {
            try {
                float t = 0;
                Vector2 currentPos = _object.transform.position;
                while (t < _duration) {
                    await Awaitable.NextFrameAsync();
                    t += Time.deltaTime;
                    float p = t / _duration;
                    Vector2 newPos = Vector2.Lerp(currentPos, _targetPos, p);
                    _object.transform.position = new Vector3(newPos.x, newPos.y, _savedZ);
                }

                Vector3 pos3 = new Vector3(_targetPos.x, _targetPos.y, _savedZ);
                _object.transform.position = pos3;
            }
            catch (Exception e) {
                //Ignored
            }
        }
    }
}
