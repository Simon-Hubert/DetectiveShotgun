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
        private static readonly int COLOR_ATTRIBUTE = Shader.PropertyToID("color");

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
            Color color = Color.white;

            Vector3 worldImpact = Camera.main.ScreenToWorldPoint(InputReader.PointerPosition);

            if (interactableManager) {
                ShotImpactSpriteProvider provider = interactableManager.GetComponent<ShotImpactSpriteProvider>();
                if (provider) {
                    go.GetComponent<SpriteRenderer>().sprite = provider.GetSprite();
                    impactVfx = provider.GetImpactVfx();
                }

                SpriteRenderer spriteRenderer = interactableManager.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    worldImpact.z = spriteRenderer.transform.position.z;
                    color = ReadPixel(spriteRenderer, worldImpact);
                }
            }
            
            go.transform.position = worldImpact;
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;

            PlayImpactVfx(vector3, impactVfx, color);
            _onShoot?.Invoke();
        }

        private static Color ReadPixel(SpriteRenderer spriteRenderer, Vector3 worldImpact)
        {
            Sprite sprite = spriteRenderer.sprite;
            if (sprite == null || sprite.texture == null) return Color.white;

            Texture2D texture = sprite.texture;
            if (!texture.isReadable) return Color.white;

            Vector3 localImpact = spriteRenderer.transform.InverseTransformPoint(worldImpact);
            Rect textureRect = sprite.textureRect;

            float normalizedX = (localImpact.x * sprite.pixelsPerUnit + sprite.pivot.x) / sprite.rect.width;
            float normalizedY = (localImpact.y * sprite.pixelsPerUnit + sprite.pivot.y) / sprite.rect.height;

            if (spriteRenderer.flipX) normalizedX = 1f - normalizedX;
            if (spriteRenderer.flipY) normalizedY = 1f - normalizedY;

            int pixelX = Mathf.Clamp(Mathf.FloorToInt(textureRect.x + normalizedX * textureRect.width), 0, texture.width - 1);
            int pixelY = Mathf.Clamp(Mathf.FloorToInt(textureRect.y + normalizedY * textureRect.height), 0, texture.height - 1);

            return texture.GetPixel(pixelX, pixelY);
        }

        private void PlayImpactVfx(Vector3 impact, VisualEffectAsset asset, Color color)
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
            attributes.SetVector3(COLOR_ATTRIBUTE, new Vector3(color.r, color.g, color.b));
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
