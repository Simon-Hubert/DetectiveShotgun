using System;
using System.Threading;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DShotgun.Dialogs
{
    public struct CharacterDialogData
    {
        public delegate void SkipCallback();
        /// <summary>
        /// Ca sera remplacé par des AnimatedSprite a terme
        /// </summary>
        public Sprite Left, Middle, Right; //TODO Remplacer par des AnimatedSprite
        public string Text, Name;
        public SOCharacterDialogProfile DialogProfile;
        /// <summary>
        /// -1 = None, 0,1,2 Left Middle Right
        /// </summary>
        public int SpeakingCharacterID;
        
        public SkipCallback Callback;
    }
    
    public class DialogFrame : ASequencable
    {
        #if UNITY_EDITOR
        [field: SerializeField] public (string, string) LeftRef { get; set; }
        [field: SerializeField] public (string, string) MiddleRef { get; set; }
        [field: SerializeField] public (string, string) RightRef { get; set; }
        
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
        
        public bool MoveNextAutomatically
        {
            get => _moveNextAutomatically;
            set => _moveNextAutomatically = value;
        }
        
        [field: SerializeField] public bool UseCustomName { get; set; }
        [field: SerializeField] public bool UseCustomDialogProfile { get; set; }
#endif
        
        [field: SerializeField] public int speakingCharacterID { get; set; }
        [field: SerializeField] public SOCharacterDialogProfile CharacterDialogProfile { get; set; }
        
        [SerializeField] private Sprite _left;
        [SerializeField] private Sprite _middle;
        [SerializeField] private Sprite _right;

        [SerializeField] private DialogsManager _dialogManager;
        
        
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
            if (MoveNextAutomatically) {
                _text += "[UneBaliseDeFin]"; //TODO mettre la bonne Balise
            }
            
            CharacterDialogData dialogData = new CharacterDialogData()
            {
                Left = _left,
                Middle = _middle,
                Right = _right,
                DialogProfile = CharacterDialogProfile,
                Name = _name,
                SpeakingCharacterID = speakingCharacterID,
                Text = _text,
                Callback = OnRequestedSkip
            };
            
            _dialogManager.CaptureDialogsInfo(dialogData);
        }

        private void OnRequestedSkip() {
            _requestedSkipCancellationTokenSource.Cancel();
        }
    }
}
