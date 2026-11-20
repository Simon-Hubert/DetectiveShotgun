using System;
using DShotgun.Sound;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using NaughtyAttributes;

public class DSoundEmitter : MonoBehaviour
{
    [SerializeField] private EventReference _soundEvent;
    public EventInstance soundInstance { get; protected set; }

    /// <summary>
    /// The sound will play, but the instance will stay alive until it is stopped
    /// </summary>
    public virtual void PlayAndPersist() {
        if (soundInstance.isValid()) Stop();
        
        soundInstance = SoundManager.PlaySound(_soundEvent);
    }
    /// <summary>
    /// The sound will play, and the instance will destroy itself once it is stopped or if the sound event is finished
    /// </summary>
    [Button]
    public virtual void Play() {
        if (soundInstance.isValid()) Stop();
        
        soundInstance = SoundManager.PlayOneShot(_soundEvent);
    }

    [Button]
    public virtual void Pause() {
        soundInstance.getPaused(out bool paused);
        soundInstance.setPaused(!paused);
    }
    
    /// <summary>
    /// Will stop the sound according to its FadeOut transition, and kill the instance
    /// </summary>
    [Button]
    public virtual void Stop() {
        if (!soundInstance.isValid()) return;
        
        soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        soundInstance.release();
    }
    /// <summary>
    /// Will stop the sound immediately, and kill the instance
    /// </summary>
    public virtual void StopNow() {
        if (!soundInstance.isValid()) return;
        
        soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundInstance.release();
    }

    private void OnDestroy() {
        Stop();
    }
    private void OnDisable() {
        Stop();
    }
}
