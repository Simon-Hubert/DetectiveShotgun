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
        [Header("Dialog Display")]
        [SerializeField] private Canvas _dialogCanvas;
        public bool DisplayDialogCanvas
        {
            get => _dialogCanvas.enabled;
            set => _dialogCanvas.enabled = value;
        }

        [Space(7)]
        [SerializeField] private Image _leftCharaRdr;
        [SerializeField] private Image _middleCharaRdr, _rightCharaRdr;
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
        
        [Space(7)]
        [SerializeField] private Image _displayTextBox;
        [SerializeField] private TextMeshProUGUI _dialogSpeakersName, _dialogText;
        [SerializeField] private Image _dialogContinueIcon;
        
        [Header("Comment Display")]
        [SerializeField] private Canvas _commentCanvas;

        public bool DisplayCommentCanvas {
            get => _commentCanvas.enabled;
            set => _commentCanvas.enabled = value;
        }
        
        [Space(7)]
        [SerializeField] private Image _commentTextBox;
        [SerializeField] private TextMeshProUGUI _commentSpeakersName, _commentText;

        [Header("Format Data")]
        [SerializeField, ReadOnly] private CharacterDialogProfile _currentProfile;
        [SerializeField] private bool dialogDialogAutoContinue = false;
        public bool DialogAutoContinue { get => dialogDialogAutoContinue; }
        
        public void DisplayDialogSprite(Sprite a_left, Sprite a_middle, Sprite a_right) {
            GetCharaRdr(CharaPos.LEFT).sprite = a_left;
            GetCharaRdr(CharaPos.MIDDLE).sprite = a_middle;
            GetCharaRdr(CharaPos.RIGHT).sprite = a_right;

            GetCharaRdr(CharaPos.LEFT).enabled = GetCharaRdr(CharaPos.LEFT).sprite != null;
            GetCharaRdr(CharaPos.MIDDLE).enabled = GetCharaRdr(CharaPos.MIDDLE).sprite != null;
            GetCharaRdr(CharaPos.RIGHT).enabled = GetCharaRdr(CharaPos.RIGHT).sprite != null;
        }
        public void DisplayDialogSpeaker(string a_name, CharaPos a_posing) {
            _dialogSpeakersName.text = a_name;
            ChangeSpeakersSprite(a_posing);
        }
        public async Awaitable DisplayDialogAsync(CancellationToken a_cancel, SOCharacterDialogProfile a_profile, string a_text) {
            try {
                ChangeDialogProfile(a_profile);
                ContinueButtonState(false);
                
                await ComputeCharByCharAsync(a_cancel, _dialogText, a_text);
                
                if (!dialogDialogAutoContinue) ContinueButtonState(true);
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Display Text Async was cancelled - {e}", gameObject);
            }
        }

        public async Awaitable DisplayCommentAsync(CancellationToken a_cancel, string a_name, SOCharacterDialogProfile a_profile, string a_text) {
            try {
                _commentSpeakersName.text = a_name;
                ChangeDialogProfile(a_profile);
                
                await ComputeCharByCharAsync(a_cancel, _commentText, a_text);
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Display Text Async was cancelled - {e}", gameObject);
            }
        }
        
        private async Awaitable ComputeCharByCharAsync(CancellationToken a_cancel, TextMeshProUGUI a_textBox, string a_text) {
            try
            {
                float delay = Time.deltaTime;
                a_textBox.text = "";
                StringBuilder a = new StringBuilder(a_textBox.text);
                
                for (int i = 0; i < a_text.Length; i++) {
                    if (a_text[i] == '<') a.Append(TagsTraitement(a_text, ref i));
                    if (string.IsNullOrWhiteSpace(a_text[i].ToString())) a.Append(a_text[i]);
                    else {
                        while (_currentProfile.typingDelay > delay) {
                            await Awaitable.NextFrameAsync(a_cancel);
                            delay += Time.deltaTime;
                        }
                        
                        delay -= _currentProfile.typingDelay;
                        
                        a.Append(a_text[i]);
                        a_textBox.text = a.ToString();
                    }
                    
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
            _dialogContinueIcon.enabled = a_show;
        }
        
        private void ChangeSpeakersSprite(CharaPos a_posing) {
            _ = TransitionSpeakerSprite(GetCharaRdr(CharaPos.LEFT), a_posing == CharaPos.LEFT ? true : false);
            _ = TransitionSpeakerSprite(GetCharaRdr(CharaPos.MIDDLE), a_posing == CharaPos.MIDDLE ? true : false);
            _ = TransitionSpeakerSprite(GetCharaRdr(CharaPos.RIGHT), a_posing == CharaPos.RIGHT ? true : false);
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

        private string TagsTraitement(string a_text, ref int r_index) {
            StringBuilder tagSB = new StringBuilder("<");
            
            for (r_index = r_index + 1; r_index < a_text.Length; r_index++) {
                tagSB.Append(a_text[r_index]);

                if (a_text[r_index] == '>') {
                    r_index++;
                    break;
                }
            }

            string tag = tagSB.ToString();

            if (tag[^1] == '>') {
                //Check custom tag

                return tag;
            }
            else {
                return "";
            }
        }
    }
}
