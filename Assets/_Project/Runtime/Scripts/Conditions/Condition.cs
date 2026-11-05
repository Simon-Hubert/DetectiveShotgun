using System;
using UnityEngine;

namespace DShotgun
{
    public interface ICondition
    {
        bool Evaluate(Blackboard blackboard);

        /// <summary>S'abonne à l'événement qui peut faire changer le résultat de <see cref="Evaluate"/>.</summary>
        void Subscribe(Action onChanged);

        void Unsubscribe(Action onChanged);
    }

    [Serializable]
    public class StateCondition : ICondition
    {
        [SerializeField] private GameState _state;

        private Action<GameState> _handler;

        public bool Evaluate(Blackboard blackboard) {
            return GameManager.CurrentState == _state;
        }

        public void Subscribe(Action onChanged) {
            Unsubscribe(onChanged);
            _handler = _ => onChanged();
            GameManager.OnStateChange += _handler;
        }

        public void Unsubscribe(Action onChanged) {
            GameManager.OnStateChange -= _handler;
        }
    }
    
    [Serializable]
    public class ItemCondition : ICondition
    {
        [SerializeField] private ItemData _item;

        public bool Evaluate(Blackboard blackboard) {
            return ShotgunManager.IsLoaded && ShotgunManager.LoadedItem.Name == _item.Name;
        }

        public void Subscribe(Action onChanged) {
            ShotgunManager.OnShotgunLoaded += onChanged;
            ShotgunManager.OnShotgunUnloaded += onChanged;
        }

        public void Unsubscribe(Action onChanged) {
            ShotgunManager.OnShotgunLoaded -= onChanged;
            ShotgunManager.OnShotgunUnloaded -= onChanged;
        }
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

        public void Subscribe(Action onChanged) {
            Blackboard.OnFlagRaised += onChanged;
        }

        public void Unsubscribe(Action onChanged) {
            Blackboard.OnFlagRaised -= onChanged;
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

        public void Subscribe(Action onChanged) {
            Condition.Subscribe(_a, onChanged);
            Condition.Subscribe(_b, onChanged);
        }

        public void Unsubscribe(Action onChanged) {
            Condition.Unsubscribe(_a, onChanged);
            Condition.Unsubscribe(_b, onChanged);
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

        public void Subscribe(Action onChanged) {
            Condition.Subscribe(_a, onChanged);
            Condition.Subscribe(_b, onChanged);
        }

        public void Unsubscribe(Action onChanged) {
            Condition.Unsubscribe(_a, onChanged);
            Condition.Unsubscribe(_b, onChanged);
        }
    }

    [Serializable]
    public class Not : ICondition
    {
        [SerializeReference, ConditionSelector] private ICondition _a;
        
        public bool Evaluate(Blackboard blackboard) {
            return _a != null && !_a.Evaluate(blackboard);
        }

        public void Subscribe(Action onChanged) {
            Condition.Subscribe(_a, onChanged);
        }

        public void Unsubscribe(Action onChanged) {
            Condition.Unsubscribe(_a, onChanged);
        }
    }

    [Serializable]
    public class Always : ICondition
    {
        public bool Evaluate(Blackboard blackboard) {
            return true;
        }

        public void Subscribe(Action onChanged) { }

        public void Unsubscribe(Action onChanged) { }
    }

    [Serializable]
    public class Never : ICondition
    {
        public bool Evaluate(Blackboard blackboard) {
            return false;
        }

        public void Subscribe(Action onChanged) { }

        public void Unsubscribe(Action onChanged) { }
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

        public void Subscribe(Action onChanged) { }

        public void Unsubscribe(Action onChanged) { }
    }

    public static class Condition
    {
        public static bool Evaluate(ICondition condition, Blackboard blackboard) {
            return condition != null && condition.Evaluate(blackboard);
        }

        public static void Subscribe(ICondition condition, Action onChanged) {
            condition?.Subscribe(onChanged);
        }

        public static void Unsubscribe(ICondition condition, Action onChanged) {
            condition?.Unsubscribe(onChanged);
        }
    }
}
