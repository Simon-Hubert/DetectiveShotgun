using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace DShotgun
{
    public abstract class AReader : MonoBehaviour
    {
        public abstract Awaitable ComputeTextAsync(string text, CancellationToken cancel);
        public abstract void Clear();
    }
    
    public class TransitionSingleton : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _gui;
        [SerializeField] private AReader _reader;
        private static TransitionSingleton instance;

        private void Awake()
        {
            if(!instance) instance = this;
            else {
                Debug.LogError("DeuxSingletons transi dans la scene");
            }
            _image.enabled = false;
        }
        
        public static void SetImage(Sprite spr)
        {
            instance._image.sprite = spr;
        }

        public static void Show()
        {
            instance._image.enabled = true;
            instance._gui.SetActive(true);
        }
        
        public static void Hide()
        {
            instance._gui.SetActive(false);
            instance._image.enabled = false;
        }

        public static void Clear()
        {
            instance._reader.Clear();
        }

        public static async Awaitable ShowTextAsync(string txt)
        {
            await instance._reader.ComputeTextAsync(txt, instance.destroyCancellationToken);
        }
    }
}
