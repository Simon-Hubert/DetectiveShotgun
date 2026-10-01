using System;
using System.Threading;
using HierarchySequences;
using UnityEngine;
using UnityEngine.UI;

namespace DShotgun.Dialogs
{
    public struct CharacterDialogData
    {
        public string Name;
        public string SpriteName;
        public bool isSpeaking;
    }
    
    public class DialogFrame : ASequencable
    {
        #if UNITY_EDITOR
        [field: SerializeField] public (string, string) LeftRef { get; set; }
        [field: SerializeField] public (string, string) MiddleRef { get; set; }
        [field: SerializeField] public (string, string) RightRef { get; set; }
        
        [field: SerializeField] public bool customName { get; set; }
        [field: SerializeField] public int speakingCharacterID { get; set; }
        
        public Sprite Left
        {
            get => _left;
            set => _left = value;
        }

        public Sprite Middle
        {
            get => _middle;
            set => _middle = value;
        }

        public Sprite Right
        {
            get => _right;
            set => _right = value;
        }

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }
#endif
        
        [SerializeField] private Sprite _left;
        [SerializeField] private Sprite _middle;
        [SerializeField] private Sprite _right;
        
        private string _text;
        private string _name;
        
        private bool _moveNextAutomatically;

        private CancellationTokenSource _requestedSkipCancellationTokenSource = new CancellationTokenSource();
        
        protected override async Awaitable OnPlay() {
            try {
                while (true) {
                    TransferData();
                    await Awaitable.NextFrameAsync(_requestedSkipCancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException oce) {
                //CleanupSiBesoing
            }
        }

        private void TransferData() {
            
        }

        private void OnRequestedSkip() {
            _requestedSkipCancellationTokenSource.Cancel();
        }
    }
}
