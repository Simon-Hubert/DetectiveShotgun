using System;
using System.Threading;
using DShotgun.RichText;
using HierarchySequences;
using UnityEngine;

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
        [Serializable]
        public struct SpriteRef
        {
            [SerializeField] public string _characterName;
            [SerializeField] public string _spriteName;
        }
        
        [field: SerializeField] public SpriteRef LeftRef { get; set; }
        [field: SerializeField] public SpriteRef MiddleRef { get; set; }
        [field: SerializeField] public SpriteRef RightRef { get; set; }
        
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
        
        [field: SerializeField] public int SpeakingCharacterID { get; set; }
        [field: SerializeField] public SOCharacterDialogProfile CharacterDialogProfile { get; set; }
        
        [SerializeField] private Sprite _left;
        [SerializeField] private Sprite _middle;
        [SerializeField] private Sprite _right;
        
        [SerializeField] private RichTextData _text;
        [SerializeField] private string _name;
        
        [SerializeField] private bool _moveNextAutomatically;

        private CancellationTokenSource _requestedSkipCancellationTokenSource;
        
        protected override async Awaitable OnPlay() {
            try
            {
                _requestedSkipCancellationTokenSource = new CancellationTokenSource();
                TransferData();
                
                while (true) {
                    await Awaitable.NextFrameAsync(_requestedSkipCancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                //throw new NotImplementedException();
            }
        }

        private void TransferData() {
            string text = RichTextCompiler.Compile(_text.Text, RichTextTarget.Tmp, RichTextTagRegistry.Default).Output;
            if (MoveNextAutomatically) {
                text += "[UneBaliseDeFin]"; //TODO mettre la bonne Balise
            }
            
            CharacterDialogData dialogData = new CharacterDialogData()
            {
                Left = _left,
                Middle = _middle,
                Right = _right,
                DialogProfile = CharacterDialogProfile,
                Name = _name,
                SpeakingCharacterID = SpeakingCharacterID,
                Text = text,
                Callback = OnRequestedSkip
            };
            
            DialogsManager.instance.CaptureDialogsInfo(dialogData);
        }

        private void OnRequestedSkip() {
            _requestedSkipCancellationTokenSource.Cancel();
        }
    }
}
