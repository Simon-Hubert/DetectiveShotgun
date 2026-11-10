using UnityEngine;
using FMODUnity;

namespace DShotgun
{
    public class ShotImpact : MonoBehaviour
    {
        private InputReader _ir;
        [SerializeField] private ImpactPool _pool;
        [SerializeField] private CallSoundOnSurface _soundOnSurface;
        [Space(7)]
        [SerializeField] private StudioEventEmitter _shootEmitter;

        private void Start()
        {
            _ir = GetComponent<InputReader>();
            if (_ir != null)
            {
                _ir.OnLeftClickPressed += Shoot;
            }
        }

        private void OnEnable()
        {
            if (!_ir) return;
            _ir.OnLeftClickPressed += Shoot;
        }

        private void OnDisable()
        {
            if (!_ir) return;
            _ir.OnLeftClickPressed -= Shoot;
        }

        private void Shoot()
        {
            _shootEmitter.Play();
            
            GameObject go = _pool.GetShotImpact();
            go.transform.position = Camera.main.ScreenToWorldPoint(InputReader.PointerPosition);
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;

            _soundOnSurface.CallSound(Camera.main.ScreenToWorldPoint(InputReader.PointerPosition));
        }
    }
}
