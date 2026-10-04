using System;
using UnityEngine;

namespace DShotgun
{
    /// <summary>
    /// /// Écoute l'<see cref="InputReader"/> et ne décide rien lui-même : les conditions et les priorités
    /// sont gérées par le manager de l'objet touché.
    /// Détecte l'objet interactif visé par le joueur et déclenche son <see cref="InteractableManager"/>.
    /// </summary>
    public class InteractableController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputReader _inputs;
        
        [SerializeField] private RichTextData _interactText;

        private void Start()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void OnEnable()
        {
            _inputs.OnLeftClickPressed += HandleLeftClickPressed;
            _inputs.OnRightClickPressed += HandleRightClickPressed;
            _inputs.OnScrolled += HandleScrolled;
        }

        private void HandleScrolled(int obj)
        {
            throw new System.NotImplementedException();
        }

        private void HandleRightClickPressed()
        {
            throw new System.NotImplementedException();
        }

        private void HandleLeftClickPressed()
        {
            Vector2 worldPoint = _camera.ScreenToWorldPoint(_inputs.PointerPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPoint);
            if (hit != null && hit.TryGetComponent(out InteractableManager interactable))
            {
                if (interactable.Interact())
                {

                    Debug.Log($"Interacted with {interactable.name}");
                }
            }
        }
    }
}
