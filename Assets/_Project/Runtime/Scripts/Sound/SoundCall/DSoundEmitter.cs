using DShotgun.Sound;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using NaughtyAttributes;
using UnityEngine.Serialization;

namespace DShotgun.Sound
{
    public class DSoundEmitter : MonoBehaviour
    {
        [SerializeField] public EventReference soundEvent;
        public EventInstance soundInstance { get; protected set; }

        public void CreateInstance() {
            SoundManager.CreateSoundInstance(soundEvent);
        }
        public void CreateInstance(EventReference a_ref) {
            SoundManager.CreateSoundInstance(soundEvent);
        }
        
        /// <summary>
        /// The sound will play, and the instance will destroy itself once it is stopped or if the sound event is finished
        /// </summary>
        [Button]
        public virtual void SoundPlay() {
            if (soundInstance.isValid()) SoundStop();
            
            soundInstance = SoundManager.PlayOneShot(soundEvent);
        }
        /// <summary>
        /// The sound will play, but the instance will stay alive until it is stopped
        /// </summary>
        public virtual void SoundPlayAndPersist() {
            if (soundInstance.isValid()) SoundStop();
            
            soundInstance = SoundManager.PlaySound(soundEvent);
        }
        /// <summary>
        /// The sound will play, but the instance will stay alive until it is stopped. The instance will be used to play sound again when this is called and the instance already exist.
        /// </summary>
        public virtual void SoundPlayRepeat()
        {
            if (soundInstance.isValid()) soundInstance.start();
            else soundInstance = SoundManager.PlaySound(soundEvent);
        }
        
        public virtual void SoundPause() {
            soundInstance.getPaused(out bool paused);
            soundInstance.setPaused(!paused);
        }
        
        /// <summary>
        /// Will stop the sound according to its FadeOut transition, and kill the instance
        /// </summary>
        [Button]
        public virtual void SoundStop() {
            if (!soundInstance.isValid()) return;
            
            soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            soundInstance.release();
        }
        /// <summary>
        /// Will stop the sound immediately, and kill the instance
        /// </summary>
        public virtual void SoundStopNow() {
            if (!soundInstance.isValid()) return;
            
            soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            soundInstance.release();
        }

        protected void OnDestroy() {
            SoundStop();
        }
        protected void OnDisable() {
            SoundStop();
        }
    }
    }
