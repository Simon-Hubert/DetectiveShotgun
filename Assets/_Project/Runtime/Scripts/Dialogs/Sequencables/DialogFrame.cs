using System;
using System.Threading;
using DShotgun.RichText;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Serializable]
        public struct SpriteRef
        {
            public int _characterId;   // 0 = personne
            public int _spriteId;

            public bool TryGetCharacter(CharactersDatabase database, out CharacterData character) {
                character = default;
                return database != null && database.TryGetCharacter(_characterId, out character);
            }
        }

        [FormerlySerializedAs("<LeftRef>k__BackingField")] [SerializeField] private SpriteRef _leftRef;
        [FormerlySerializedAs("<MiddleRef>k__BackingField")] [SerializeField] private SpriteRef _middleRef;
        [FormerlySerializedAs("<RightRef>k__BackingField")] [SerializeField] private SpriteRef _rightRef;
        [FormerlySerializedAs("<SpeakingCharacterID>k__BackingField")] [SerializeField] private int _speakingCharacterID;

        [SerializeField] private RichTextData _text;
        [SerializeField] private bool _moveNextAutomatically;

        // Surcharges : lues seulement si la case correspondante est cochée
        [FormerlySerializedAs("<UseCustomName>k__BackingField")] [SerializeField] private bool _useCustomName;
        [SerializeField] private string _name;
        [FormerlySerializedAs("<UseCustomDialogProfile>k__BackingField")] [SerializeField] private bool _useCustomDialogProfile;
        [FormerlySerializedAs("<CharacterDialogProfile>k__BackingField")] [SerializeField] private SOCharacterDialogProfile _characterDialogProfile;

        private CancellationTokenSource _requestedSkipCancellationTokenSource;

        private SpriteRef GetRef(int position) => position switch {
            0 => _leftRef,
            1 => _middleRef,
            2 => _rightRef,
            _ => default
        };

        private bool TryGetSpeaker(CharactersDatabase database, out CharacterData speaker) => GetRef(_speakingCharacterID).TryGetCharacter(database, out speaker);

        private string ResolveName(CharactersDatabase database) {
            if (_useCustomName) return _name;
            if (TryGetSpeaker(database, out CharacterData speaker)) return speaker.Name;
            return string.Empty;
        }

        private static Sprite ResolveSprite(CharactersDatabase database, SpriteRef spriteRef) {
            if (!spriteRef.TryGetCharacter(database, out CharacterData character)) return null;
            return character.GetSprite(spriteRef._spriteId);
        }

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
            CharactersDatabase database = CharactersDatabase.Load();
            if (database == null) Debug.LogError($"'{name}' : CharactersDatabase introuvable (Resources/{CharactersDatabase.RESOURCE_PATH})", gameObject);

            string text = RichTextCompiler.Correct(_text.Text, RichTextTagRegistry.Default);
            if (_moveNextAutomatically) {
                text += "<pass>"; //TODO mettre la bonne Balise
            }

            SOCharacterDialogProfile profile = _characterDialogProfile;
            if (!_useCustomDialogProfile) {
                profile = null;
                if (TryGetSpeaker(database, out CharacterData speaker)) profile = speaker.DialogProfile;
            }

            CharacterDialogData dialogData = new CharacterDialogData()
            {
                Left = ResolveSprite(database, _leftRef),
                Middle = ResolveSprite(database, _middleRef),
                Right = ResolveSprite(database, _rightRef),
                DialogProfile = profile,
                Name = ResolveName(database),
                SpeakingCharacterID = _speakingCharacterID,
                Text = text,
                Callback = OnRequestedSkip
            };

            DialogsManager.instance.CaptureDialogsInfo(dialogData);
        }

        public void OnRequestedSkip() {
            _requestedSkipCancellationTokenSource.Cancel();
        }

        protected override string GetName()
        {
            return ResolveName(CharactersDatabase.Load()) + " Speaking";
        }

#if UNITY_EDITOR
        // Sans surcharge, rien d'autre que le texte et les IDs n'est sérialisé
        private void OnValidate() {
            if (!_useCustomName) _name = null;
            if (!_useCustomDialogProfile) _characterDialogProfile = null;
        }

        protected override float Hue => Animator.StringToHash(ResolveName(CharactersDatabase.Load()));
#endif
    }
}
