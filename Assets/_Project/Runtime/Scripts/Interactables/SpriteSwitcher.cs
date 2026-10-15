using System;
using UnityEngine;

namespace DShotgun
{
    public class SpriteSwitcher : MonoBehaviour
    {
        private SpriteRenderer _sr;

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public void SwitchTo(Sprite sprite) => _sr.sprite = sprite;
    }
}
