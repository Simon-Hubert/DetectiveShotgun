using System;
using System.Threading;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DShotgun.Dialogs
{
    public class DialogsDisplayer : ATextDisplayer
    {
        [Header("Characters Images")]
        [SerializeField] protected Image _leftCharaRdr;
        [SerializeField] protected Image _middleCharaRdr;
        [SerializeField] protected Image _rightCharaRdr;
        protected Image GetCharaImg(CharaPos a_posing) {
            //Debug.Log($"Change Speaker Sprite {a_posing.ToString()}");
            switch (a_posing) {
                case CharaPos.LEFT:
                    return _leftCharaRdr;
                case CharaPos.MIDDLE:
                    return _middleCharaRdr;
                case CharaPos.RIGHT:
                    return _rightCharaRdr;
                default :
                    return null;
            }
        }
        protected EventTrigger _leftCharaTrigr, _middleCharaTrigr, _rightCharaTrigr;
        protected EventTrigger GetCharaTrigr(CharaPos a_posing) {
            switch (a_posing) {
                case CharaPos.LEFT:
                    return _leftCharaTrigr;
                case CharaPos.MIDDLE:
                    return _middleCharaTrigr;
                case CharaPos.RIGHT:
                    return _rightCharaTrigr;
                default :
                    return null;
            }
        }
        protected Vector3 _leftCharaPos, _middleCharaPos, _rightCharaPos;
        protected Vector3 GetCharaPos(CharaPos a_posing) {
            switch (a_posing) {
                case CharaPos.LEFT:
                    return _leftCharaPos;
                case CharaPos.MIDDLE:
                    return _middleCharaPos;
                case CharaPos.RIGHT:
                    return _rightCharaPos;
                default :
                    return Vector3.zero;
            }
        }

        [Header("Textbox")]
        [SerializeField] protected Image _dialogContinueIcon;
        [SerializeField] protected Image _leftNameSprite, _rightNameSprite;
        [SerializeField] protected Sprite _blankNameSprite;
        [SerializeField] protected TextMeshProUGUI _leftNameText, _rightNameText;
        protected Vector2 _leftNamePos, _rightNamePos;
        [SerializeField] protected Vector2 _nameSpriteOffset = new Vector2(0, -20.0f);

        protected bool _skipShotCharacter = false;

        protected void Start() {
            _leftNamePos = _leftNameSprite.transform.position;
            _rightNamePos = _rightNameSprite.transform.position;

            _leftCharaTrigr = _leftCharaRdr ? _leftCharaRdr.GetComponent<EventTrigger>() : null;
            _middleCharaTrigr = _middleCharaRdr ? _middleCharaRdr.GetComponent<EventTrigger>() : null;
            _rightCharaTrigr = _rightCharaRdr ? _rightCharaRdr.GetComponent<EventTrigger>() : null;
        }

        public override bool ShowCanvas {
            get => _displayParent.activeSelf;
            set {
                _displayState = value ? TextDisplayState.Idle : TextDisplayState.Closed;
                _displayParent.SetActive(value);

                if (GameManager.CurrentState == GameState.Transition) return;
                GameManager.SwitchState(value ? GameState.Dialogue : GameState.Playing);
            }
        }
        
        public override async Awaitable DisplayAsync(CharacterDialogData a_dialogData, CancellationToken a_cancel) {
            try {
                if (_displayState == TextDisplayState.Closed)
                    ShowCanvas = true;
                
                DisplaySpeaker(a_dialogData);
                
                ContinueButtonState(false);
                await Awaitable.NextFrameAsync(a_cancel);
                
                _displayState = TextDisplayState.Writing;
                _skipShotCharacter = false;
                await _dumbReader.ComputeTextAsync(a_dialogData.Text,  GetProfile(a_dialogData), a_cancel);

                if (!_dumbReader.HasAutoPassed && !_skipShotCharacter) {
                    ContinueButtonState(true);
                    _displayState = TextDisplayState.Waiting;
                }
                else {
                    _displayState = TextDisplayState.Idle;
                }
                
                while (_displayState == TextDisplayState.Waiting) {
                    await Awaitable.NextFrameAsync(a_cancel);
                }
            }
            catch (OperationCanceledException) {
                Debug.Log($"'{name}' : Compute Dialog Async was cancelled by token", gameObject);
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Compute Dialog Async was cancelled - {e}", gameObject);
            }
            finally {
                if (_displayState != TextDisplayState.Closed)
                    a_dialogData.Callback();
            }
        }

        public void ActionTextBox() {
            if (_displayState == TextDisplayState.Writing) _dumbReader.InstantDisplayAll();
            if (_displayState == TextDisplayState.Waiting) _displayState = TextDisplayState.Idle;
        }
        public void ActionCharacterBox() {
            _dumbReader.InstantDisplayAll();
            _skipShotCharacter = true;
        }

        protected void ContinueButtonState(bool a_show) {
            _dialogContinueIcon.enabled = a_show;
        }
        
        protected void DisplaySpeaker(CharacterDialogData a_dialogData) {
            SOCharacterDialogProfile profile = GetProfile(a_dialogData);
            
            _rightNameText.text = "";
            _leftNameText.text = "";
            
            switch ((CharaPos)a_dialogData.SpeakingCharacterID) { 
                case CharaPos.RIGHT :
                    _rightNameSprite.sprite = profile.CharaNameSprite;
                    _leftNameSprite.sprite = _blankNameSprite;
                    
                    _rightNameSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    _leftNameSprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    
                    _rightNameText.text = profile.NameDisplayedAsTMP ? a_dialogData.Name : "";

                    _leftNameSprite.transform.position = _leftNamePos + _nameSpriteOffset;
                    _rightNameSprite.transform.position = _rightNamePos;
                    break;
                
                default :
                    _leftNameSprite.sprite = profile.CharaNameSprite;
                    _rightNameSprite.sprite = _blankNameSprite;

                    _leftNameSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    _rightNameSprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                    _leftNameText.text = profile.NameDisplayedAsTMP ? a_dialogData.Name : "";
                    
                    _rightNameSprite.transform.position = _rightNamePos + _nameSpriteOffset;
                    _leftNameSprite.transform.position = _leftNamePos;
                    break;
            }
            
            _textBox.sprite = profile.TextBoxSprite;
            
            _ = TransitionSpeakerSprite(CharaPos.LEFT, a_dialogData.Left, a_dialogData.SpeakingCharacterID == (int)CharaPos.LEFT ? true : false);
            _ = TransitionSpeakerSprite(CharaPos.MIDDLE, a_dialogData.Middle, a_dialogData.SpeakingCharacterID == (int)CharaPos.MIDDLE ? true : false);
            _ = TransitionSpeakerSprite(CharaPos.RIGHT, a_dialogData.Right, a_dialogData.SpeakingCharacterID == (int)CharaPos.RIGHT ? true : false);
        }

        protected async Awaitable TransitionSpeakerSprite(CharaPos a_pos, Sprite a_sprite, bool a_isSpeaker) {
            try {
                Image img = GetCharaImg(a_pos);
                GetCharaTrigr(a_pos).enabled = a_isSpeaker;
                
                
                if (a_sprite == null) {
                    img.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    img.sprite = a_sprite;
                }
                else if (a_isSpeaker) {
                    img.color = Color.white;
                    img.sprite = a_sprite;
                }
                else {
                    img.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    img.sprite = a_sprite;
                }
            }
            catch (OperationCanceledException) {
                Debug.LogWarning($"'{name}' : Change Speaker Sprite '{a_pos.ToString()}' Async was cancelled", gameObject);
            }
        }
    }
}
