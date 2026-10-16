using UnityEngine;

namespace DShotgun
{
    public class InteractableManagerProxy : InteractableManager
    {
        [SerializeField] private InteractableManager _im;

        public override bool Aim()
        {
            return _im.Aim();
        }

        public override bool Interact()
        {
            return _im.Interact();
        }
    }
}
