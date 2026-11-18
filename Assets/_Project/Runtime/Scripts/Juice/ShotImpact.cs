using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Events;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("_woodImpactVfx"), SerializeField, Tooltip("Joué quand l'objet tiré n'a pas de ShotImpactSpriteProvider ou pas de VFX, et quand on ne touche aucun objet")]
        private VisualEffect _defaultImpactVfx;
        [SerializeField, Tooltip("Distance de l'œil virtuel du tireur devant la planche : plus elle est petite, plus les éclats partent de biais sur les bords de l'écran")]
        private float _shooterDistance = 6f;

        private static readonly int IMPACT_EVENT = Shader.PropertyToID("OnImpact");
        private static readonly int POSITION_ATTRIBUTE = Shader.PropertyToID("position");
        private static readonly int DIRECTION_ATTRIBUTE = Shader.PropertyToID("direction");
        private readonly Dictionary<VisualEffectAsset, VisualEffect> _impactVfxByAsset = new Dictionary<VisualEffectAsset, VisualEffect>();

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
            VisualEffectAsset impactVfx = null;

            if (interactableManager) {
                ShotImpactSpriteProvider provider = interactableManager.GetComponent<ShotImpactSpriteProvider>();
                if (provider) {
                    if (provider.GetSprite() != null) go.GetComponent<SpriteRenderer>().sprite = provider.GetSprite();
                    impactVfx = provider.GetImpactVfx();
                }
            }
            
            go.transform.position = Camera.main.ScreenToWorldPoint(InputReader.PointerPosition);
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;

            PlayImpactVfx(vector3, impactVfx);
            _onShoot?.Invoke();
        }

        private void PlayImpactVfx(Vector3 impact, VisualEffectAsset asset)
        {
            VisualEffect vfx = GetImpactVfx(asset);
            if (vfx == null) return;

            // normale vers la cam genre shotgun est toujours au centre de la scene
            Vector3 eye = Camera.main.transform.position;
            Vector3 incoming = (impact - new Vector3(eye.x, eye.y, -_shooterDistance)).normalized;
            Vector3 ejection = new Vector3(incoming.x, incoming.y, -incoming.z);

            VFXEventAttribute attributes = vfx.CreateVFXEventAttribute();
            attributes.SetVector3(POSITION_ATTRIBUTE, impact);
            attributes.SetVector3(DIRECTION_ATTRIBUTE, ejection);
            vfx.SendEvent(IMPACT_EVENT, attributes);
        }

        //mise en cache
        private VisualEffect GetImpactVfx(VisualEffectAsset asset)
        {
            if (asset == null) return _defaultImpactVfx;
            if (_impactVfxByAsset.TryGetValue(asset, out VisualEffect vfx) && vfx != null) return vfx;

            vfx = null;
            foreach (VisualEffect child in GetComponentsInChildren<VisualEffect>(true))
            {
                if (child.visualEffectAsset != asset) continue;
                vfx = child;
                break;
            }
            if (vfx == null) vfx = CreateImpactVfx(asset);
            _impactVfxByAsset[asset] = vfx;
            return vfx;
        }

        private VisualEffect CreateImpactVfx(VisualEffectAsset asset)
        {
            Transform parent = transform;
            if (_defaultImpactVfx != null) parent = _defaultImpactVfx.transform.parent;

            GameObject vfxObject = new GameObject(asset.name);
            vfxObject.transform.SetParent(parent, false);
            VisualEffect vfx = vfxObject.AddComponent<VisualEffect>();
            vfx.initialEventName = "";
            vfx.visualEffectAsset = asset;
            if (_defaultImpactVfx != null)
            {
                Renderer defaultRenderer = _defaultImpactVfx.GetComponent<Renderer>();
                Renderer vfxRenderer = vfxObject.GetComponent<Renderer>();
                vfxRenderer.sortingLayerID = defaultRenderer.sortingLayerID;
                vfxRenderer.sortingOrder = defaultRenderer.sortingOrder;
            }
            return vfx;
        }
    }
}
