using DShotgun.Sound;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using NaughtyAttributes;
using UnityEngine.Serialization;

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
    public virtual void Play() {
        if (soundInstance.isValid()) Stop();
        
        soundInstance = SoundManager.PlayOneShot(soundEvent);
    }
    /// <summary>
    /// The sound will play, but the instance will stay alive until it is stopped
    /// </summary>
    public virtual void PlayAndPersist() {
        if (soundInstance.isValid()) Stop();
        
        soundInstance = SoundManager.PlaySound(soundEvent);
    }
    /// <summary>
    /// The sound will play, but the instance will stay alive until it is stopped. The instance will be used to play sound again when this is called and the instance already exist.
    /// </summary>
    public virtual void PlayRepeat()
    {
        if (soundInstance.isValid()) soundInstance.start();
        else soundInstance = SoundManager.PlaySound(soundEvent);
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
