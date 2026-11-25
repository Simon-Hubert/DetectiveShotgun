using UnityEngine;

namespace DShotgun
{
    public class ShotImpactSpriteOverride : AShotImpactOverride<Sprite>
    {
        public override void Override(GameObject shotImpact)
        {
            shotImpact.GetComponent<SpriteRenderer>().sprite = _ref;
        }
    }
}
