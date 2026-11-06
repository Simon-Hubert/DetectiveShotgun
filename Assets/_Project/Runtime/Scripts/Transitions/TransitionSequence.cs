using HierarchySequences;
using UnityEngine;

namespace DShotgun
{
    public class TransitionSequence : ASequencable
    {
        [SerializeField] private ATransition _transition;
        [SerializeField] private AView _to;
        
        
        protected override async Awaitable OnPlay()
        {
            await _transition.TransitionAsync(Camera.main, _to);
        }
    }
}
