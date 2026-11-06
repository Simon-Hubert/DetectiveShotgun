using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading;
using NaughtyAttributes;
using UnityEngine.Serialization;

namespace DShotgun.Dialogs
{
    /// <summary>
    /// Abstract Class made to write text in a designated canvas & textbox
    /// </summary>
    public abstract class ATextDisplayer : MonoBehaviour
    {
        [SerializeField] protected GameObject _displayParent;
        public virtual bool ShowCanvas
        {
            get => _displayParent.activeSelf;
            set {
                _displayState = value ? TextDisplayState.Idle : TextDisplayState.Closed;
                _displayParent.SetActive(value);
            }
        }
        /// <summary>
        /// Default Text customization profile
        /// </summary>
        [SerializeField] protected SOCharacterDialogProfile _defaultProfile;
        protected SOCharacterDialogProfile GetProfile(CharacterDialogData a_dialogData) => a_dialogData.DialogProfile == null ? _defaultProfile : a_dialogData.DialogProfile;

        /// <summary>
        /// Current status of the displayer
        /// </summary>
        [Space(7)]
        [SerializeField, ReadOnly] protected TextDisplayState _displayState = TextDisplayState.Idle;
        public TextDisplayState GetDisplayState { get => _displayState; }
        
        protected TextDumbReader _dumbReader = new TextDumbReader();
        
        [Header("Textbox")]
        [SerializeField] protected Image _textBox;
        [SerializeField] protected TextMeshProUGUI _speakersName, _mainText;
        
        protected void Awake() {
            _dumbReader = new TextDumbReader(_mainText);
        }

        /// <summary>
        /// Method to call when you need to display the text
        /// </summary>
        /// <param name="a_dialogData"></param>
        /// <param name="a_cancel"></param>
        public virtual async Awaitable DisplayAsync(CharacterDialogData a_dialogData, CancellationToken a_cancel) {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public enum TextDisplayState {
        Closed = -1,
        Idle = 0,
        Writing = 1,
        Waiting = 2
    }
}
