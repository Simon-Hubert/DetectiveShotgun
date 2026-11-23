using System;
using UnityEngine;
using UnityEngine.UI;

namespace DShotgun
{
    public class OverlaySingleton : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private AnimationCurve _fadeCurve;
        public static OverlaySingleton instance;

        private void Awake() {
            DontDestroyOnLoad(transform.parent);
            if(!instance) instance = this;
            else {
                Debug.LogError("DeuxSingletons dans la scene");
            }
            _image.enabled = false;
        }

        public static void SetColor(Color color)
        {
            instance._image.color = color;
        }
        
        public static void SetOpacity(float a)
        {
            var color = instance._image.color;
            color.a = a;
            instance._image.color = color;
        }

        public static void Show()
        {
            instance._image.enabled = true;
        }
        
        public static void Hide()
        {
            instance._image.enabled = false;
        }

        public async Awaitable FadeToBlackTransitionAsync(float duration)
        {
            try {
                float t = 0f;
                SetColor(Color.black);
                SetOpacity(0);
                Show();
                while (t < duration) {
                    await Awaitable.NextFrameAsync();
                    t += Time.deltaTime;
                    float p = t / duration;
                    p = _fadeCurve.Evaluate(p);
                    SetOpacity(p);
                }

                SetOpacity(1);
            }
            catch (Exception e) {
                
            }
        }
        
        public async Awaitable FadeInTransitionAsync(float duration)
        {
            try {
                float t = 0f;
                Show();
                SetColor(Color.black);
                SetOpacity(1);
                while (t < duration) {
                    await Awaitable.NextFrameAsync();
                    t += Time.deltaTime;
                    float p = t / duration;
                    p = 1 - p;
                    p = _fadeCurve.Evaluate(p);
                    SetOpacity(p);
                }
                SetOpacity(0);
                Hide();
                
            }
            catch (Exception e) {
                throw e;
            }
        }
    }
}
