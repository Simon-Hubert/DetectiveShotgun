using System;
using UnityEngine;

namespace DShotgun
{
    public abstract class ATransition : ScriptableObject
    {
        public delegate void OnEndCallback();
        
        protected abstract Awaitable DoTransitionAsync(Camera cam, AView to);

        public async Awaitable TransitionAsync(Camera cam, AView to, OnEndCallback onEndCallback = null) {
            try
            {
                await DoTransitionAsync(cam, to);
                onEndCallback();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Transition(Camera cam, AView to, OnEndCallback onEndCallback = null) {
            _ = TransitionAsync(cam, to, onEndCallback);
        }
    }
}
