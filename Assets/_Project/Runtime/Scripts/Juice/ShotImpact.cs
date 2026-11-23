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
        
        private const int SEARCH_RADIUS = 12;


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
            if(interactableManager == null) return;
            GameObject go = _pool.GetShotImpact();
            List<VisualEffectAsset> impactVfxs = null;
            Color color = Color.white;

            Vector3 worldImpact = Camera.main.ScreenToWorldPoint(InputReader.PointerPosition);
            
            ShotImpactSpriteProvider provider = interactableManager.GetComponent<ShotImpactSpriteProvider>();
            if (provider) 
            {
                go.GetComponent<SpriteRenderer>().sprite = provider.GetSprite();
                impactVfxs = provider.GetImpactVfxs();
            }

            SpriteRenderer spriteRenderer = interactableManager.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                worldImpact.z = spriteRenderer.transform.position.z;
                color = ReadPixel(spriteRenderer, worldImpact);
            }
            
            go.transform.position = worldImpact;
            Vector3 vector3 = go.transform.position;
            vector3.z = 0;
            go.transform.position = vector3;

            foreach (VisualEffectAsset impactVfx in impactVfxs)
            {
                PlayImpactVfx(vector3, impactVfx, color);
            }
            _onShoot?.Invoke();
        }
        
        private static Color ReadPixel(SpriteRenderer spriteRenderer, Vector3 worldImpact)
        {
            Sprite sprite = spriteRenderer.sprite;
            if (sprite == null || sprite.texture == null) return Color.white;

            Texture2D texture = sprite.texture;
            if (!texture.isReadable)
            {
                Debug.Log($"[SHOTIMPACT] Texture {texture.name} is not readable.");
                return Color.white;
            }
            Vector3 localImpact = spriteRenderer.transform.InverseTransformPoint(worldImpact);
            if (spriteRenderer.flipX) localImpact.x = -localImpact.x;
            if (spriteRenderer.flipY) localImpact.y = -localImpact.y;

            Vector2 inSprite = (Vector2)localImpact * sprite.pixelsPerUnit + sprite.pivot;
            Rect textureRect = sprite.textureRect;
            int centerX = Mathf.FloorToInt(textureRect.x - sprite.textureRectOffset.x + inSprite.x);
            int centerY = Mathf.FloorToInt(textureRect.y - sprite.textureRectOffset.y + inSprite.y);

            for (int radius = 0; radius <= SEARCH_RADIUS; radius++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != radius) continue;
                        int x = centerX + dx;
                        int y = centerY + dy;
                        if (x < textureRect.xMin || x >= textureRect.xMax || y < textureRect.yMin || y >= textureRect.yMax) continue;
                        Color pixel = texture.GetPixel(x, y);
                        if (pixel.a < 0.5f) continue;
                        return pixel.linear;
                    }
                }
            }
            return Color.white;
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
