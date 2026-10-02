using System;
using HierarchySequences;
using UnityEngine;

namespace DShotgun
{
    public class RaiseFlagSequence : ASequencable
    {
        [SerializeField] private FlagReference _flag;
        public event Action RaiseFlagEnded;

        protected override async Awaitable OnPlay()
        {
            try {
                Blackboard.Instance.RaiseFlag(_flag.FlagName);

            }
            catch (OperationCanceledException oce) {

            }
            finally {
                RaiseFlagEnded?.Invoke();
            }
        }
    }
}
