using System;
using System.Collections.Generic;
using UnityEngine;

namespace DShotgun
{
    public class PendingItem : MonoBehaviour
    {
        [SerializeReference, ConditionSelector] private ICondition _condition;
        
        
        private void Awake()
        {
            gameObject.SetActive(false);
            Blackboard.OnFlagRaised += Check;
        }

        void Check()
        {
            if (_condition.Evaluate(Blackboard.Instance))
            {
                gameObject.SetActive(true);
            }
        }
    }
}