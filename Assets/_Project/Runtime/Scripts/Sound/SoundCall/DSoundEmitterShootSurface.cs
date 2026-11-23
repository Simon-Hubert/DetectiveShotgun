using DShotgun.Sound;
using FMODUnity;
using UnityEngine;

public class DSoundEmitterShootSurface : DSoundEmitter
{
    [SerializeField] private ShootSurfaces _surface;
    
    public override void Play() {
        soundInstance = SoundManager.PlayShootSurface(_surface);
    }

    public void PlayAtGameobject(GameObject a_gameobject) {
        soundInstance = SoundManager.PlayShootSurface(_surface);
        RuntimeManager.AttachInstanceToGameObject(soundInstance, a_gameobject);
    }
}
