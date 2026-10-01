using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DShotgun
{
    /// <summary>
    /// Gère la réception d'un input (clic) sur l'objet interactif.
    /// Évalue ses conditions sur le <see cref="Blackboard"/> de façon ordonnée.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractableManager : MonoBehaviour
    {
        [SerializeField] private List<InteractionCase> _interactionCases = new List<InteractionCase>();

        /// <summary>
        /// Traite une interaction du joueur avec cet objet.
        /// Parcourt les cas dans l'ordre de la liste (l'ordre fait la priorité) et s'arrête au premier
        /// dont la condition est vraie sur le <see cref="Blackboard"/> : les cas suivants ne sont pas évalués.
        /// </summary>
        /// <returns>true si un cas a été validé, false si aucun ne l'est.</returns>
        public bool Interact()
        {
            foreach (var interactionCase in _interactionCases)
            {
                if(interactionCase.Evaluate(Blackboard.Instance))
                {
                    return true;
                }
            }

            return false;
        }
    
    }   
}
