using System;
using UnityEngine;

namespace DShotgun
{
    public class BackgroundAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private float _frameRate;

        private int _currentFrame = 0;
        private float _timePerFrame;
        private SpriteRenderer _sr;
        
        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (!_sr) return;
            _timePerFrame = 1f / _frameRate;
            _ = Animate();
        }

        private async Awaitable Animate()
        {
            try
            { 
                while (true)
                {
                    _sr.sprite = _sprites[_currentFrame];
                    _currentFrame++;
                    _currentFrame %= _sprites.Length;
                    await Awaitable.WaitForSecondsAsync(_timePerFrame, destroyCancellationToken);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
