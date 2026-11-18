using UnityEngine;

namespace DShotgun
{
    public class ShotImpactSpriteProvider : MonoBehaviour
    {
        [SerializeField] private Sprite _sprite;

        public virtual Sprite GetSprite()
        {
            return _sprite;
        }
    }
}
