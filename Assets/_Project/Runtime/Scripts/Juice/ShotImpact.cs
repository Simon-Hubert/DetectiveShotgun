using UnityEngine;
using FMODUnity;
using UnityEngine.Events;
using UnityEngine.VFX;

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
        [Space(7)]
        [SerializeField] private VisualEffect _woodImpactVfx;
        [SerializeField, Tooltip("Distance de l'œil virtuel du tireur devant la planche : plus elle est petite, plus les éclats partent de biais sur les bords de l'écran")]
        private float _shooterDistance = 6f;

        private static readonly int IMPACT_EVENT = Shader.PropertyToID("OnImpact");
        private static readonly int POSITION_ATTRIBUTE = Shader.PropertyToID("position");
        private static readonly int DIRECTION_ATTRIBUTE = Shader.PropertyToID("direction");
        private static readonly string[] NON_WOOD_TAGS = { "Metal", "Window", "Glass" };
        private VFXEventAttribute _impactAttributes;

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

            PlayWoodImpact(vector3);
            _onShoot?.Invoke();
        }

        private void PlayWoodImpact(Vector3 impact)
        {
            if (_woodImpactVfx == null) return;

            Collider2D surface = Physics2D.OverlapPoint(impact);
            if (surface != null)
            {
                foreach (string tag in NON_WOOD_TAGS)
                {
                    if (surface.CompareTag(tag)) return;
                }
            }
            Vector3 eye = Camera.main.transform.position;
            Vector3 incoming = (impact - new Vector3(eye.x, eye.y, -_shooterDistance)).normalized;
            Vector3 ejection = new Vector3(incoming.x, incoming.y, -incoming.z);

            if (_impactAttributes == null) _impactAttributes = _woodImpactVfx.CreateVFXEventAttribute();
            _impactAttributes.SetVector3(POSITION_ATTRIBUTE, impact);
            _impactAttributes.SetVector3(DIRECTION_ATTRIBUTE, ejection);
            _woodImpactVfx.SendEvent(IMPACT_EVENT, _impactAttributes);
        }
    }
}
