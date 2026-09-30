using System;
using UnityEngine;

namespace DetectiveShotgun.Conditions
{
    public interface ICondition
    {
        bool Evaluate(Blackboard blackboard);
    }

    [Serializable]
    public class FlagCondition : ICondition
    {
        [SerializeField] private FlagReference _flag;

        public bool Evaluate(Blackboard blackboard) {
            return blackboard != null
                   && !string.IsNullOrEmpty(_flag.FlagName)
                   && blackboard.Flags.TryGetValue(_flag.FlagName, out bool value)
                   && value;
        }
    }

    [Serializable]
    public class And : ICondition
    {
        [SerializeReference, ConditionSelector] private ICondition _a;
        [SerializeReference, ConditionSelector] private ICondition _b;

        public bool Evaluate(Blackboard blackboard) {
            return Condition.Evaluate(_a, blackboard) && Condition.Evaluate(_b, blackboard);
        }
    }

    [Serializable]
    public class Or : ICondition
    {
        [SerializeReference, ConditionSelector] private ICondition _a;
        [SerializeReference, ConditionSelector] private ICondition _b;

        public bool Evaluate(Blackboard blackboard) {
            return Condition.Evaluate(_a, blackboard) || Condition.Evaluate(_b, blackboard);
        }
    }

    [Serializable]
    public class Not : ICondition
    {
        [SerializeReference, ConditionSelector] private ICondition _a;
        
        public bool Evaluate(Blackboard blackboard) {
            return _a != null && !_a.Evaluate(blackboard);
        }
    }

    [Serializable]
    public class Always : ICondition
    {
        public bool Evaluate(Blackboard blackboard) {
            return true;
        }
    }

    [Serializable]
    public class Never : ICondition
    {
        public bool Evaluate(Blackboard blackboard) {
            return false;
        }
    }

    [Serializable]
    public class EvaluatableRef : ICondition
    {
        [SerializeField] private AEvaluatable _evaluatable;

        public bool Evaluate(Blackboard blackboard) {
            // À remplacer par la ligne ci-dessous une fois `public abstract bool Evaluate();` ajouté à AEvaluatable :
            // return _evaluatable != null && _evaluatable.Evaluate();
            throw new NotImplementedException();
        }
    }

    public static class Condition
    {
        public static bool Evaluate(ICondition condition, Blackboard blackboard) {
            return condition != null && condition.Evaluate(blackboard);
        }
    }
}
