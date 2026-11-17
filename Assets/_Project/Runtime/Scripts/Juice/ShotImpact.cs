using UnityEngine;
using FMODUnity;
using UnityEngine.Events;

namespace DShotgun
{
    public class ShotImpact : MonoBehaviour
    {
        private InteractableController _ic;
        [SerializeField] private ImpactPool _pool;
        [SerializeField] private UnityEvent _onShoot;
        [SerializeField] private CallSoundOnSurface _soundOnSurface;
        [Space(7)]
        [SerializeField] private StudioEventEmitter _shootEmitter;

        private void Start()
        {
            _ic = GetComponent<InteractableController>();
            if (_ic != null)
            {
                _ic.OnInteractableInteracted += Shoot;
            }
        }

        private void OnEnable()
        {
            if (!_ic) return;
            _ic.OnInteractableInteracted += Shoot;
        }

        private void OnDisable()
        {
            if (!_ic) return;
            _ic.OnInteractableInteracted -= Shoot;
        }

        private void Shoot(InteractableManager interactableManager)
        {
            GameObject go = _pool.GetShotImpact();
            
            
            if (interactableManager) {
                ShotImpactSpriteProvider sisp = interactableManager.GetComponent<ShotImpactSpriteProvider>();
                if (sisp) {
                    go.GetComponent<SpriteRenderer>().sprite = sisp.GetSprite();
                }
            }
            
            go.transform.position = Camera.main.ScreenToWorldPoint(InputReader.PointerPosition);
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;
            
            _onShoot?.Invoke();
        }
    }
}
