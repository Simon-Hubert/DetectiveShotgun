using UnityEngine;
using UnityEngine.VFX;

namespace DShotgun
{
    public class ShotImpactSpriteProvider : MonoBehaviour
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField, Tooltip("Effet joué quand on tire sur cet objet (vide = effet par défaut du ShotImpact)")]
        private VisualEffectAsset _impactVfx;

        public virtual Sprite GetSprite()
        {
            return _sprite;
        }

        public virtual VisualEffectAsset GetImpactVfx()
        {
            return _impactVfx;
        }
    }
}
