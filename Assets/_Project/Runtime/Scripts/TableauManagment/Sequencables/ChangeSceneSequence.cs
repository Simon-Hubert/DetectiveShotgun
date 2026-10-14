using HierarchySequences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DShotgun
{
    public class ChangeSceneSequence : ASequencable
    {
        [SerializeField] private int _tableau;
        [SerializeField] private int _state;
        
        protected override async Awaitable OnPlay()
        {
            TableauManager.LoadTableau(_tableau, _state);
        }
    }
}
