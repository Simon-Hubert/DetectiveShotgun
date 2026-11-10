using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun
{
    public class CallSoundOnSurface : MonoBehaviour
    {
        [SerializeField] private EventReference _fmodEventRef;

        [SerializeField, ReadOnly] private string[] _surfaces =
        {
            "wood",
            "window",
            "glass",
            "metal"
        };

        public void CallSound(Vector2 a_worldPoint) {
            RaycastHit2D rh = Physics2D.Raycast(a_worldPoint, Vector2.zero);
            if (rh != null)
            {
                for (int i = 0; i < _surfaces.Length; i++) {
                    if (_surfaces[i].ToLower() == rh.collider.tag.ToLower())
                    {
                        EventInstance fmodEvent = RuntimeManager.CreateInstance(_fmodEventRef);
                        RuntimeManager.AttachInstanceToGameObject(fmodEvent, gameObject);
                        
                        fmodEvent.setParameterByName("Surface", i);
                        fmodEvent.start();
                        fmodEvent.release();
                    }
                }
            }
        }
    }
}
