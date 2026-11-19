using System;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
        
        private FMOD.Studio.Bus _masterBus;
        private FMOD.Studio.Bus _sfxMixer;
        private FMOD.Studio.Bus _interfaceMixer;
        private FMOD.Studio.Bus _musicMixer;
        
        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = Mathf.Clamp01(value);
        }
        [SerializeField, Range(0, 1)] private float _masterVolume = 1;
        public float SFXVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Mathf.Clamp01(value);
        }
        [SerializeField, Range(0, 1)] private float _sfxVolume = 1;
        public float InterfaceVolume
        {
            get => _interfaceVolume;
            set => _interfaceVolume = Mathf.Clamp01(value);
        }
        [SerializeField, Range(0, 1)] private float _interfaceVolume = 1;
        public float MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = Mathf.Clamp01(value);
        }
        [SerializeField, Range(0, 1)] private float _musicVolume = 1;

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
        }

        private void Start() {
            _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            _sfxMixer = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            _interfaceMixer = FMODUnity.RuntimeManager.GetBus("bus:/Interface");
            _musicMixer = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        }

        private void Update() {
            _masterBus.setVolume(MasterVolume);
            _sfxMixer.setVolume(SFXVolume);
            _interfaceMixer.setVolume(InterfaceVolume);
            _musicMixer.setVolume(MusicVolume);
        }
    }
}
