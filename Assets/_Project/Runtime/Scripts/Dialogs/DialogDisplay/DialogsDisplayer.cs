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
        [SerializeField] private Button _leftCharaRdr;
        [SerializeField] private Button _middleCharaRdr;
        [SerializeField] private Button _rightCharaRdr;
        private Image GetCharaImg(CharaPos a_posing) {
            Debug.Log($"Change Speaker Sprite {a_posing.ToString()}");
            switch (a_posing) {
                case CharaPos.LEFT:
                    return _leftCharaRdr.image;
                case CharaPos.MIDDLE:
                    return _middleCharaRdr.image;
                case CharaPos.RIGHT:
                    return _rightCharaRdr.image;
                default :
                    return null;
            }
        }
        private Button GetCharaBtn(CharaPos a_posing) {
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
        private Vector3 _leftCharaPos, _middleCharaPos, _rightCharaPos;
        private Vector3 GetCharaPos(CharaPos a_posing) {
            switch (a_posing) {
                case CharaPos.LEFT:
                    return _leftCharaPos;
                case CharaPos.RIGHT:
                    return _rightCharaPos;
                default :
                    return Vector3.zero;
            }
        }

        [Header("Textbox")]
        [SerializeField] private Image _dialogContinueIcon;
        [SerializeField] private Image _leftNameSprite, _rightNameSprite;
        [SerializeField] private Sprite _blankNameSprite;
        [SerializeField] private TextMeshProUGUI _leftNameText, _rightNameText;
        private Vector2 _leftNamePos, _rightNamePos;
        [SerializeField] private Vector2 _nameSpriteOffset = new Vector2(0, -20.0f);

        private bool _skipShotCharacter = false;

        private void Start() {
            _leftNamePos = _leftNameSprite.transform.position;
            _rightNamePos = _rightNameSprite.transform.position;
        }

        public override bool ShowCanvas
        {
            get => _displayParent.activeSelf;
            set {
                _displayState = value ? TextDisplayState.Idle : TextDisplayState.Closed;
                _displayParent.SetActive(value);
                
                
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

        private void ContinueButtonState(bool a_show) {
            _dialogContinueIcon.enabled = a_show;
        }
        
        public void DisplaySpeaker(CharacterDialogData a_dialogData) {
            _rightNameText.text = "";
            _leftNameText.text = "";
            
            switch ((CharaPos)a_dialogData.SpeakingCharacterID) { 
                case CharaPos.RIGHT :
                    _rightNameSprite.sprite = a_dialogData.DialogProfile.CharaNameSprite;
                    _leftNameSprite.sprite = a_dialogData.Left != null ? _blankNameSprite : null;
                    
                    _rightNameSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    _leftNameSprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    
                    _rightNameText.text = a_dialogData.DialogProfile.NameDisplayedAsTMP ? a_dialogData.Name : "";

                    _leftNameSprite.transform.position = _leftNamePos + _nameSpriteOffset;
                    _rightNameSprite.transform.position = _rightNamePos;
                    break;
                
                default :
                    _leftNameSprite.sprite = a_dialogData.DialogProfile.CharaNameSprite;
                    _rightNameSprite.sprite = a_dialogData.Right != null ? _blankNameSprite : null;

                    _leftNameSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    _rightNameSprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                    _leftNameText.text = a_dialogData.DialogProfile.NameDisplayedAsTMP ? a_dialogData.Name : "";
                    
                    _rightNameSprite.transform.position = _rightNamePos + _nameSpriteOffset;
                    _leftNameSprite.transform.position = _leftNamePos;
                    break;
            }
            
            _ = TransitionSpeakerSprite(GetCharaImg(CharaPos.LEFT), a_dialogData.Left, a_dialogData.SpeakingCharacterID == (int)CharaPos.LEFT ? true : false);
            _ = TransitionSpeakerSprite(GetCharaImg(CharaPos.MIDDLE), a_dialogData.Middle, a_dialogData.SpeakingCharacterID == (int)CharaPos.MIDDLE ? true : false);
            _ = TransitionSpeakerSprite(GetCharaImg(CharaPos.RIGHT), a_dialogData.Right, a_dialogData.SpeakingCharacterID == (int)CharaPos.RIGHT ? true : false);
        }

        private async Awaitable TransitionSpeakerSprite(Image a_img, Sprite a_sprite, bool a_isSpeaker) {
            try {
                Debug.Log($"'{name}' : Change Speaker Sprite '{a_img.name}' - Sprite '{a_sprite != null}' - Is speaker '{a_isSpeaker}'");
                
                if (a_sprite == null) {
                    a_img.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    a_img.sprite = a_sprite;
                }
                else if (a_isSpeaker) {
                    a_img.color = Color.white;
                    a_img.sprite = a_sprite;
                }
                else {
                    a_img.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    a_img.sprite = a_sprite;
                }
            }
            catch (OperationCanceledException) {
                Debug.LogWarning($"'{name}' : Change Speaker Sprite '{a_img.name}' Async was cancelled", gameObject);
            }
        }
    }
}
