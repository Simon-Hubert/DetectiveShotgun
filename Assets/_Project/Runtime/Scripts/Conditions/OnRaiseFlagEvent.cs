using UnityEngine;
using UnityEngine.Events;

namespace DShotgun
{
    public class OnRaiseFlagEvent : MonoBehaviour
    {
        [SerializeReference, ConditionSelector] private ICondition _condition;
        [SerializeField] private UnityEvent _onConditionTrue;
        
        
        private void Awake()
        {
            Blackboard.OnFlagRaised += Check;
        }

        void Check()
        {
            if (_condition.Evaluate(Blackboard.Instance))
            {
                _onConditionTrue?.Invoke();
            }
        }
    }
}
