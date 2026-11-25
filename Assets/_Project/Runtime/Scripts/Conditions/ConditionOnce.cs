using System;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public class ConditionOnce : ICondition
    {
        private bool _result = true;
        
        public bool Evaluate(Blackboard blackboard)
        {
            if (!_result) return false;
            _result = false;
            return true;
        }

        public void Subscribe(Action onChanged) { }

        public void Unsubscribe(Action onChanged) { }
    }
}
