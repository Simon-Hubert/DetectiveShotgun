using System.Collections.Generic;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Events;

namespace DShotgun
{
    /// <summary>
    /// Gère la réception d'un input (clic) sur l'objet interactif.
    /// Évalue ses conditions sur le <see cref="Blackboard"/> de façon ordonnée.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractableManager : MonoBehaviour
    {
        [SerializeField] private int _priority;
        [SerializeField] private List<InteractionCase> _interactionCases = new List<InteractionCase>();
        [SerializeField] private List<InteractionCase> _aimCases = new List<InteractionCase>();
        
        [SerializeField] private UnityEvent _onInteracted;
        [SerializeField] private UnityEvent _onAimed;

        public int Priority => _priority;

        /// <summary>
        /// Traite une interaction du joueur avec cet objet.
        /// Parcourt les cas dans l'ordre de la liste (l'ordre fait la priorité) et s'arrête au premier
        /// dont la condition est vraie sur le <see cref="Blackboard"/> : les cas suivants ne sont pas évalués.
        /// </summary>
        /// <returns>true si un cas a été validé, false si aucun ne l'est.</returns>
        virtual public bool Interact()
        {
            try
            {
                foreach (InteractionCase interactionCase in _interactionCases)
                {
                    if (interactionCase.Evaluate(Blackboard.Instance)) return true;
                }
                return false;
            }
            finally
            {
                _onInteracted?.Invoke();
            }
        }

        /// <summary>
        /// Traite le fait que le joueur vise cet objet.
        /// </summary>
        virtual public bool  Aim()
        {
            bool result = false;
            foreach (InteractionCase aimCase in _aimCases)
            {
                if (aimCase.Evaluate(Blackboard.Instance))
                {
                    result = true;
                    break;
                }
            }

            _onAimed?.Invoke();
            return result;
        }
    }
}
