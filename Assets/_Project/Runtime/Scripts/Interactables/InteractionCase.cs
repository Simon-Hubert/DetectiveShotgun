using System;
using DetectiveShotgun.Conditions;
using UnityEngine;

[Serializable]
public class InteractionCase
{
    [SerializeReference, ConditionSelector] private ICondition _condition;
    public bool Evaluate(Blackboard blackboard)
    {
        return _condition.Evaluate(blackboard);
    }
}
