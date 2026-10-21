using System;
using System.Threading;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DShotgun.Dialogs
{
    public class DialogsDisplayer : ATextDisplayer
    {
        [Header("Characters Images")]
        [SerializeField] private Image _leftCharaRdr;
        [SerializeField] private Image _rightCharaRdr;
        private Image GetCharaRdr(CharaPos a_posing) {
            switch (a_posing) {
                case CharaPos.LEFT:
                    return _leftCharaRdr;
                case CharaPos.RIGHT:
                    return _rightCharaRdr;
                default :
                    return null;
            }
        }
        private Vector3 _leftCharaPos, _rightCharaPos;
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

        [Header("Data")]
        [SerializeField, ReadOnly] private bool dialogDialogAutoContinue = false;
        public bool DialogAutoContinue { get => dialogDialogAutoContinue; }
        
        public void DisplayDialogSprite(Sprite a_left, Sprite a_right) {
            GetCharaRdr(CharaPos.LEFT).sprite = a_left;
            GetCharaRdr(CharaPos.RIGHT).sprite = a_right;
        }
        public void DisplayDialogSpeaker(string a_name, CharaPos a_posing) {
            _speakersName.text = a_name;
            ChangeSpeakersSprite(a_posing);
        }
        public override async Awaitable DisplayAsync(CharacterDialogData a_dialogData, CancellationToken a_cancel) {
            try {
                if (_displayState == TextDisplayState.Closed)
                    ShowCanvas = true;
                
                DisplayDialogSprite(a_dialogData.Left, a_dialogData.Right);
                DisplayDialogSpeaker(a_dialogData.Name, (CharaPos)a_dialogData.SpeakingCharacterID);
                
                ContinueButtonState(false);

                _displayState = TextDisplayState.Writing;
                await _dumbReader.ComputeCharByCharAsync(a_dialogData.Text,  GetProfile(a_dialogData), a_cancel);

                if (!dialogDialogAutoContinue) {
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
        public void ContinueDialog() {
            if (_displayState == TextDisplayState.Waiting) {
                _displayState = TextDisplayState.Idle;
            }
        }

        private void ContinueButtonState(bool a_show) {
            _dialogContinueIcon.enabled = a_show;
        }
        
        private void ChangeSpeakersSprite(CharaPos a_posing) {
            _ = TransitionSpeakerSprite(GetCharaRdr(CharaPos.LEFT), a_posing == CharaPos.LEFT ? true : false);
            _ = TransitionSpeakerSprite(GetCharaRdr(CharaPos.RIGHT), a_posing == CharaPos.RIGHT ? true : false);
        }

        private async Awaitable TransitionSpeakerSprite(Image a_sprite, bool a_isSpeaker) {
            try {
                if (a_sprite.sprite == null) {
                    a_sprite.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
                else if (a_isSpeaker) {
                    a_sprite.color = Color.white;
                }
                else {
                    a_sprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                }
            }
            catch (OperationCanceledException) {
                Debug.LogWarning($"'{name}' : Change Speaker Sprite '{a_sprite.name}' Async was cancelled", gameObject);
            }
        }
    }
}
