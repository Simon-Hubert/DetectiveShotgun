using HierarchySequences;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DShotgun
{
    public class ChangeSceneSequence : ASequencable
    {
        [SerializeField, Scene] private int _tableau;
        [SerializeField, Scene] private int _state;
        
        protected override async Awaitable OnPlay()
        {
            TableauManager.LoadTableau(_tableau, _state);
        }
    }
}
