using UnityEngine;

namespace DShotgun
{
    public class SpriteSwitcher : MonoBehaviour
    {
        private SpriteRenderer _sr;

        public void SwitchTo(Sprite sprite) => _sr.sprite = sprite;
    }
}
