using UnityEngine;
using UnityEngine.UI;

namespace DShotgun
{
    public class SetImageAlpha : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetAlpha(float a)
        {
            Color color = _image.color;
            color.a = a;
            _image.color = color;
        }
    }
}
