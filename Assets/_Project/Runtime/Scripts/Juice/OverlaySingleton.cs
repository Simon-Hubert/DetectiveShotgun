using UnityEngine;
using UnityEngine.UI;

namespace DShotgun
{
    public class OverlaySingleton : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private static OverlaySingleton _instance;

        private void Awake() {
            if (!_instance) {
                _instance = this;
            }
            else {
                Destroy(this);
            }
            _image.enabled = false;
        }

        public static void SetColor(Color color)
        {
            _instance._image.color = color;
        }
        
        public static void SetOpacity(float a)
        {
            var color = _instance._image.color;
            color.a = a;
            _instance._image.color = color;
        }

        public static void Show()
        {
            _instance._image.enabled = true;
        }
        
        public static void Hide()
        {
            _instance._image.enabled = false;
        }
    }
}
