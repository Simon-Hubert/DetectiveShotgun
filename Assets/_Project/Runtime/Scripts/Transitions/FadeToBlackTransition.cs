using System;
using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(fileName = "FadeToBlack", menuName = "DetectiveShotgun/Transitions/FadeToBlack")]
    public class FadeToBlackTransition : ATransition
    {
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _fadeIn;
        [SerializeField] private AnimationCurve _fadeOut;
        [SerializeField, Range(0, 1)] private float _centerPoint = 0.5f;
        
        protected override async Awaitable DoTransitionAsync(Camera cam, AView to)
        {
            try
            {
                float t = 0f;
                float centerPoint = _duration * _centerPoint;
                OverlaySingleton.SetColor(Color.black);
                OverlaySingleton.SetOpacity(0);
                OverlaySingleton.Show();
                while (t < centerPoint)
                {
                    await Awaitable.NextFrameAsync();
                    t += Time.deltaTime;
                    float p = t / centerPoint;
                    p = _fadeIn.Evaluate(p);
                    OverlaySingleton.SetOpacity(p);
                }
                OverlaySingleton.SetOpacity(1);
                CameraHelpers.ApplyConfiguration(cam, to.GetConfiguration());
                float end = _duration - centerPoint;
                t = 0;
                while (t < end)
                {
                    await Awaitable.NextFrameAsync();
                    t += Time.deltaTime;
                    float p = t / end;
                    p = _fadeOut.Evaluate(1 - p);
                    OverlaySingleton.SetOpacity(p);
                }
                
                OverlaySingleton.Hide();
                OverlaySingleton.SetOpacity(0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
