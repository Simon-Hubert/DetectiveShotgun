using System;
using System.Threading;
using UnityEngine;

namespace DShotgun.Sound
{
    /// <summary>
    /// N'est pas vouer a exister indefiniment, seulement la en placeholder en attendant que se soit fait sur FMod
    /// </summary>
    public class DSoundEmitterCarAlarm : DSoundEmitter
    {
        [Header("Fades")]
        [SerializeField] private float _alarmFadeDuration;
        [SerializeField] private AnimationCurve _alarmFade;
        [Space(5)]
        [SerializeField] private float _alarmOffDuration;
        [SerializeField] private AnimationCurve _alarmShuttingOff;

        private CancellationTokenSource _tokenSource;

        public override void SoundPlayAndPersist() {
            base.SoundPlayAndPersist();
            _tokenSource = new CancellationTokenSource();
            _ = AlarmFadeAsync(_tokenSource.Token);
        }

        private async Awaitable AlarmFadeAsync(CancellationToken a_cancel) {
            try {
                Debug.Log($"ALARM Fade");
                
                float current = 0;

                while (current < _alarmFadeDuration) {
                    soundInstance.setVolume(_alarmFade.Evaluate(current / _alarmFadeDuration));

                    await Awaitable.NextFrameAsync(a_cancel);
                    current += Time.deltaTime;
                }
                
                soundInstance.setVolume(_alarmFade.Evaluate(1.0f));
            }
            catch (Exception e){
                Debug.LogError(e);
            }
        }

        public void SoundStopAlarm() {
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested) {
                _tokenSource.Cancel();
                _ = AlarmFadeOffAsync();
            }
        }
        private async Awaitable AlarmFadeOffAsync() {
            try {
                Debug.Log($"ALARM Fade Off");
                
                float current = 0;

                while (current < _alarmOffDuration) {
                    //soundInstance.setVolume(_alarmShuttingOff.Evaluate(current / _alarmOffDuration));
                    soundInstance.setPitch(_alarmShuttingOff.Evaluate(current / _alarmOffDuration));

                    await Awaitable.NextFrameAsync();
                    current += Time.deltaTime;
                }
                
                SoundStop();
            }
            catch (Exception e){
                Debug.LogError(e);
            }
        } 
    }
}
