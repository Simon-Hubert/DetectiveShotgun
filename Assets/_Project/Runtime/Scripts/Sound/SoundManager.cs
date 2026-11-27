using System;
using FMODUnity;
using FMOD.Studio;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun.Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
        
        private Bus _masterBus;
        private Bus _sfxMixer;
        private Bus _interfaceMixer;
        private Bus _musicMixer;
        private bool _busInit = false;
        
        #region Volumes
        [Header("Volumes")]
        public float MasterVolume {
            get => _masterVolume;
            set {
                _masterVolume = Mathf.Clamp01(value);
                UpdateVolume();
            }
        }
        [SerializeField, Range(0, 1)] private float _masterVolume = 1;
        public float SFXVolume {
            get => _sfxVolume;
            set {
                _sfxVolume = Mathf.Clamp01(value);
                UpdateVolume();
            }
        }
        [SerializeField, Range(0, 1)] private float _sfxVolume = 1;
        public float InterfaceVolume {
            get => _interfaceVolume;
            set {
                _interfaceVolume = Mathf.Clamp01(value);
                UpdateVolume();
            }
        }
        [SerializeField, Range(0, 1)] private float _interfaceVolume = 1;
        public float MusicVolume {
            get => _musicVolume;
            set {
                _musicVolume = Mathf.Clamp01(value);
                UpdateVolume();
            }
        }
        [SerializeField, Range(0, 1)] private float _musicVolume = 1;
        #endregion

        [Header("Sound References")] [SerializeField]
        private EventReference _eventShootSurface;
        
        private void OnValidate() {
            if (Application.isPlaying) UpdateVolume();
        }

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
        }
        private void Start() {
            _masterBus = RuntimeManager.GetBus("bus:/");
            _sfxMixer = RuntimeManager.GetBus("bus:/SFX");
            _interfaceMixer = RuntimeManager.GetBus("bus:/Interface");
            _musicMixer = RuntimeManager.GetBus("bus:/Music");

            _busInit = true;
            UpdateVolume();
        }

        private void UpdateVolume() {
            if (!_busInit) {
                Debug.LogWarning($"'{name}' : Volume Update was called but buses aren't initialized yet.");
                return;
            }
            
            _masterBus.setVolume(MasterVolume);
            _sfxMixer.setVolume(SFXVolume);
            _interfaceMixer.setVolume(InterfaceVolume);
            _musicMixer.setVolume(MusicVolume);
        }

        public static PARAMETER_DESCRIPTION[] GetAllParam(EventInstance a_instance)
        {
            if (!a_instance.isValid()) return new PARAMETER_DESCRIPTION[0];
            
            a_instance.getDescription(out EventDescription eventDesc);

            eventDesc.getParameterDescriptionCount(out int cunt);
            PARAMETER_DESCRIPTION[] result = new PARAMETER_DESCRIPTION[cunt];
            
            for (int i = 0; i < cunt; i++)
            {
                eventDesc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paramDesc);
                result[i] = paramDesc;
            }

            return result;
        }
        public static PARAMETER_ID GetParamID(EventInstance a_instance, string a_parameterName)
        {
            if (!a_instance.isValid()) return new PARAMETER_ID();
            
            a_instance.getDescription(out EventDescription eventDesc);
            eventDesc.getParameterDescriptionByName(a_parameterName, out PARAMETER_DESCRIPTION paramDesc);
            return paramDesc.id;
        }
        public static bool IsValidParamID(PARAMETER_ID a_id) {
            if (a_id.data1 == 0 && a_id.data2 == 0) 
                return false;
            else 
                return true;
        }

        public static EventInstance CreateSoundInstance(EventReference a_ref) {
            if (a_ref.IsNull) {
                Debug.LogWarning($"SoundManager : tried to CreateInstance but given EventReference is null");
                return default;
            }
            
            return RuntimeManager.CreateInstance(a_ref);
        }
        
        #region CallSoundMethods

        public static EventInstance PlayShootSurface(ShootSurfaces a_surface) {
            if (a_surface <= ShootSurfaces.None) {
                Debug.LogWarning($"SoundManager : tried to Play Sound ShootSurface but given surface is invalid - {(int)a_surface} '{a_surface.ToString()}'");
                return default;
            }
            
            EventInstance instance = RuntimeManager.CreateInstance(SoundManager.instance._eventShootSurface);
            instance.setParameterByName("Surface", (int)a_surface);

            instance.start();
            instance.release();

            return instance;
        }

        public static EventInstance PlayOneShot(EventReference a_ref) {
            EventInstance instance = PlaySound(a_ref);
            instance.release();

            return instance;
        }

        public static EventInstance PlaySound(EventReference a_ref) {
            if (a_ref.IsNull) {
                Debug.LogWarning($"SoundManager : tried to PlaySound but given EventReference is null");
                return default;
            }
            
            EventInstance instance = RuntimeManager.CreateInstance(a_ref);
            
            instance.start();
            return instance;
        }
        
        #endregion
    }
}
