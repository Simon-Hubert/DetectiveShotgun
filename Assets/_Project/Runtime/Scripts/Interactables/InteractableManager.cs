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

        private bool _canBeAimed = true;
        private bool _canBeShot = true;

        /// <summary>
        /// Traite une interaction du joueur avec cet objet.
        /// Parcourt les cas dans l'ordre de la liste (l'ordre fait la priorité) et s'arrête au premier
        /// dont la condition est vraie sur le <see cref="Blackboard"/> : les cas suivants ne sont pas évalués.
        /// Si aucun cas n'est validé, joue l'événement par défaut <c>_onInteracted</c> ; si un cas est validé, lui seul est joué.
        /// </summary>
        /// <returns>true si un cas a été validé, false si aucun ne l'est.</returns>
        virtual public bool Interact()
        {
            if (!_canBeShot) return false;

            foreach (InteractionCase interactionCase in _interactionCases)
            {
                if (interactionCase.Evaluate(Blackboard.Instance)) return true;
            }

            _onInteracted?.Invoke();
            return false;
        }

        /// <summary>
        /// Traite le fait que le joueur vise cet objet.
        /// </summary>
        virtual public bool Aim()
        {
            if (!_canBeAimed) return false;
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
        
        /// <summary>
        /// Empeche les intéractions de Aim
        /// </summary>
        public void DisableAim()
        {
            _canBeAimed = false;
        }
        
        /// <summary>
        /// Rends possible les intéractions de Aim
        /// </summary>
        public void EnableAim()
        {
            _canBeAimed = true;
        }
        
        /// <summary>
        /// Empeche les intéractions de Shoot
        /// </summary>
        public void DisableShoot()
        {
            _canBeShot = false;
        }
        
        /// <summary>
        /// Rends possible les intéractions de Shoot
        /// </summary>
        public void EnableShoot()
        {
            _canBeShot = true;
        }
    }
}
