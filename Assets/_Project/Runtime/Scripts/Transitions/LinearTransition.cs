using System;
using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(fileName = "LinearTransition", menuName = "DetectiveShotgun/Transitions/LinearTransition")]
    public class LinearTransition : ATransition
    {
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;
        
        protected override async Awaitable DoTransitionAsync(Camera cam, AView to) {
            try
            {
                CameraConfiguration toConfig = to.GetConfiguration();
                toConfig.Yaw = Mathf.DeltaAngle(0, toConfig.Yaw);
                CameraConfiguration fromConfig = new CameraConfiguration()
                {
                    Distance = 0,
                    Fov = cam.fieldOfView,
                    Pitch = Mathf.DeltaAngle(0, cam.transform.rotation.eulerAngles.x),
                    Yaw = Mathf.DeltaAngle(0, cam.transform.rotation.eulerAngles.y),
                    Roll = Mathf.DeltaAngle(0, cam.transform.rotation.eulerAngles.z),
                    Pivot = cam.transform.position,
                };
                CameraHelpers.ApplyConfiguration(cam, fromConfig);
                float t = 0;
                while (t < _duration)
                {
                    t += Time.deltaTime;
                    float p = t / _duration;
                    p = _curve.Evaluate(p);
                    CameraHelpers.ApplyConfiguration(cam, CameraHelpers.Lerp(fromConfig, toConfig, p));
                    await Awaitable.NextFrameAsync();
                }

                CameraHelpers.ApplyConfiguration(cam, toConfig);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
