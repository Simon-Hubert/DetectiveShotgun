using System;
using System.Text;
using System.Threading;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DShotgun.Dialogs
{
    public class DialogsDisplaying : MonoBehaviour
    {
        [SerializeField] private Image _leftCharaRdr, _middleCharaRdr, _rightCharaRdr;
        private Image GetCharaRdr(CharaPos a_posing) {
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
                case CharaPos.MIDDLE:
                    return _middleCharaPos;
                case CharaPos.RIGHT:
                    return _rightCharaPos;
                default :
                    return Vector3.zero;
            }
        }

        [Header("TextBox Display")]
        [SerializeField] private Image _textBox;
        [SerializeField] private TextMeshProUGUI _speakersName, _dialog;
        [SerializeField] private Image _continueIcon;

        [Header("Format Data")]
        [SerializeField, ReadOnly] private CharacterDialogProfile _currentProfile;
        [SerializeField] private bool _autoContinue = false;
        public bool AutoContinue { get => _autoContinue; }

        public void DisplaySprite(Sprite a_left, Sprite a_middle, Sprite a_right) {
            GetCharaRdr(CharaPos.LEFT).sprite = a_left;
            GetCharaRdr(CharaPos.LEFT).sprite = a_middle;
            GetCharaRdr(CharaPos.LEFT).sprite = a_right;
        }
        public void DisplaySpeaker(string a_name, CharaPos a_posing) {
            _speakersName.text = a_name;
            ChangeSpeakersSprite(a_posing);
        }
        public async Awaitable DisplayTextAsync(CancellationToken a_cancel, SOCharacterDialogProfile a_profile, string a_text) {
            try
            {
                ContinueButtonState(false);

                await ComputeCharByCharAsync(a_cancel, a_text);

                if (!_autoContinue) ContinueButtonState(true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"'{name}' : Display Text Async was cancelled - {e}", gameObject);
            }
        }
        private async Awaitable ComputeCharByCharAsync(CancellationToken a_cancel, string a_text) {
            try
            {
                float delay = Time.deltaTime;
                _dialog.text = "";
                StringBuilder a = new StringBuilder(_dialog.text);
                
                for (int i = 0; i < a_text.Length; i++) {
                    if (_currentProfile.typingDelay > delay) {
                        await Awaitable.NextFrameAsync(a_cancel);
                        delay += Time.deltaTime;
                    }

                    delay -= _currentProfile.typingDelay;
                    
                    a.Append(a_text[i]);
                    _dialog.text = a.ToString();
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Display Text Async was cancelled - {e}", gameObject);
            }
        }
        
        private void ChangeDialogProfile(SOCharacterDialogProfile a_profile) {
            _currentProfile = new CharacterDialogProfile(a_profile);
        }
        private void ContinueButtonState(bool a_show) {
            _continueIcon.enabled = a_show;
        }

        private void ChangeSpeakersSprite(CharaPos a_posing) {
            TransitionSpeakerSprite(GetCharaRdr(CharaPos.LEFT), a_posing == CharaPos.LEFT ? true : false);
            TransitionSpeakerSprite(GetCharaRdr(CharaPos.MIDDLE), a_posing == CharaPos.MIDDLE ? true : false);
            TransitionSpeakerSprite(GetCharaRdr(CharaPos.RIGHT), a_posing == CharaPos.RIGHT ? true : false);
        }

        private async Awaitable TransitionSpeakerSprite(Image a_sprite, bool a_isSpeaker) {
            try {
                if (a_isSpeaker) {
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
