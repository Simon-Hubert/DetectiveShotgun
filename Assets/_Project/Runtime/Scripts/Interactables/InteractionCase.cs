using System;
using DetectiveShotgun.Conditions;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Un cas d'interaction d'un <see cref="InteractableManager"/> : une condition sur les flags du
/// <see cref="Blackboard"/>. Le manager parcourt ses cas dans l'ordre de la liste (l'ordre fait la priorité)
/// et s'arrête au premier dont la condition est vraie. Cette classe ne contient que des données et
/// l'évaluation de sa propre condition : le choix du cas revient au manager.
/// </summary>
[Serializable]
public class InteractionCase
{
    [SerializeReference, ConditionSelector] private ICondition _condition;
    [SerializeField] private UnityEvent _action;
    
    /// <summary>Indique si la condition de ce cas est vraie d'après l'état courant du <see cref="Blackboard"/>.</summary>
    /// <param name="blackboard">Le blackboard sur lequel évaluer la condition.</param>
    public bool Evaluate(Blackboard blackboard)
    {
        if (_condition.Evaluate(blackboard))
        {
            _action?.Invoke();
            return true;
        }

        return false;
    }
}
