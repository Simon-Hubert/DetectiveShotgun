using System;
using HierarchySequences;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DShotgun
{
    [Serializable]
    public struct TransitionSceneData
    {
        public string Date;
        public string Heure;
        public string Lieu;
        public Sprite Background;
    }
    
    public class ChangeSceneSequence : ASequencable
    {
        [SerializeField] private TransitionSceneData _data;
        [SerializeField, Scene] private int _tableau;
        [SerializeField, Scene] private int _state;
        
        protected override async Awaitable OnPlay()
        {
            TableauManager.LoadTableau(_tableau, _state);
        }
    }
}
