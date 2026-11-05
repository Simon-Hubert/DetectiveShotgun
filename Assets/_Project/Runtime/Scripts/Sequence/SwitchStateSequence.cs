using System;
using HierarchySequences;
using UnityEngine;

namespace DShotgun
{
    public class SwitchStateSequence : ASequencable
    {
        [SerializeField] private GameState _state;
        protected override async Awaitable OnPlay()
        {
            
            try {
                GameManager.SwitchState(_state);

            }
            catch (OperationCanceledException oce) {
                
            }
            finally {
               
            }
        }
    }
}
