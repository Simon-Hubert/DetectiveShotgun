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
            Condition.Subscribe(_condition, Check);
        }

        private void OnDestroy()
        {
            Condition.Unsubscribe(_condition, Check);
        }

        void Check()
        {
            if (Condition.Evaluate(_condition, Blackboard.Instance))
            {
                gameObject.SetActive(true);
                Condition.Unsubscribe(_condition, Check);
            }
        }
    }
}