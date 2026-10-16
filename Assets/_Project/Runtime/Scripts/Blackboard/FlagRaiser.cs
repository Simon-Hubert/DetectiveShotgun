using UnityEngine;

namespace DShotgun
{
    public class FlagRaiser : MonoBehaviour
    {
        [SerializeField] private FlagReference _flag;
        
        public void RaiseFlag()
        {
            Blackboard.Instance.RaiseFlag(_flag.FlagName);
        }
    }
}
