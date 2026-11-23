using System;
using HierarchySequences;
using UnityEngine;

namespace DShotgun
{
    public class TransitionSequence : ASequencable
    {
        [SerializeField] private ATransition _transition;
        [SerializeField] private AView _to;

        public static event Action<Vector2> OnCamTargetChanged ;

        protected override async Awaitable OnPlay()
        {
            GameManager.SwitchState(GameState.Transition);
            await _transition.TransitionAsync(Camera.main, _to, ChangeCamTarget);
            GameManager.SwitchState(GameState.Playing);
            
        }

        private void ChangeCamTarget()
        {
            Debug.Log("SequenceEndCallback");
            OnCamTargetChanged?.Invoke(_to.transform.position);
        }
    }
}
