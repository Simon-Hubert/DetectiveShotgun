using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace DShotgun
{
    [Obsolete("Use AShotImpactOverride<T> instead")]
    public class ShotImpactSpriteProvider : MonoBehaviour
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField, Tooltip("Effet joué quand on tire sur cet objet (vide = effet par défaut du ShotImpact)")]
        private List<VisualEffectAsset> _impactVfxs;

        public virtual Sprite GetSprite()
        {
            return _sprite;
        }

        public virtual List<VisualEffectAsset> GetImpactVfxs()
        {
            return _impactVfxs;
        }
    }
}
