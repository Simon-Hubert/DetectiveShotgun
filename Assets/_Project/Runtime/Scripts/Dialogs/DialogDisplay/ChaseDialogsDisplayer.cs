using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

namespace DShotgun.Dialogs
{
    public class ChaseDialogsDisplayer : CommentDisplayer
    {
        [Header("Textbox")] 
        [SerializeField] protected Image _leftNameSprite;
        [SerializeField] protected Image _rightNameSprite;
        [Space(7)]
        [SerializeField] protected Sprite _blankNameSprite;
        
        [SerializeField] protected TextMeshProUGUI _leftNameText, _rightNameText;
        protected Vector2 _leftNamePos, _rightNamePos;
        [Space(7)]
        [SerializeField] protected Vector2 _nameSpriteOffset = new Vector2(0, -20.0f);
        [Space(10)]
        [SerializeField] private SOCharacterDialogProfile _detectiveDialogProfile;

        protected void Start() {
            _leftNamePos = _leftNameSprite.transform.position;
            _rightNamePos = _rightNameSprite.transform.position;
        }

        public override async Awaitable DisplayAsync(CharacterDialogData a_dialogData, CancellationToken a_cancel)
        {
            try {
                if (_displayState == TextDisplayState.Closed)
                    ShowCanvas = true;
                
                for (_index = 0; _index < _listOfTextID.Count; _index++) {
                    a_dialogData = _listOfTextID[_index].dialogData;
                    if (_speakersName) _speakersName.text = "Détective Shotgun";
                    DisplaySpeaker(a_dialogData);
                
                    _displayState = TextDisplayState.Writing;
                    await _dumbReader.ComputeTextAsync(a_dialogData.Text, GetProfile(a_dialogData), a_cancel);

                    if (!_dumbReader.HasAutoPassed) {
                        _displayState = TextDisplayState.Idle;
                        await Awaitable.WaitForSecondsAsync(_defaultTimeToHide, a_cancel);
                    }
                }

                GroupID = 0;
                ShowCanvas = false;
            }
            catch (OperationCanceledException) {
                Debug.Log($"'{name}' : Compute Comment Async was cancelled by token", gameObject);
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Compute Comment Async was cancelled - {e}", gameObject);
            }
            finally {
                
            }
        }

        protected void DisplaySpeaker(CharacterDialogData a_dialogData)
        {
            Debug.Log("BITE");
            SOCharacterDialogProfile profile = GetProfile(a_dialogData);
            
            _rightNameText.text = "";
            _leftNameText.text = "";

            if (a_dialogData.DialogProfile == _detectiveDialogProfile) {
                Debug.Log("BITE GAUCHE");
                _rightNameSprite.sprite = profile.CharaNameSprite;
                _leftNameSprite.sprite = _blankNameSprite;

                _rightNameSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                _leftNameSprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                _rightNameText.text = profile.NameDisplayedAsTMP ? a_dialogData.Name : "";

                _leftNameSprite.transform.position = _leftNamePos + _nameSpriteOffset;
                _rightNameSprite.transform.position = _rightNamePos;
            }
            else{
                Debug.Log("BITE DROITE");
                _leftNameSprite.sprite = profile.CharaNameSprite;
                _rightNameSprite.sprite = _blankNameSprite;

                _leftNameSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                _rightNameSprite.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                _leftNameText.text = profile.NameDisplayedAsTMP ? a_dialogData.Name : "";
                
                _rightNameSprite.transform.position = _rightNamePos + _nameSpriteOffset;
                _leftNameSprite.transform.position = _leftNamePos;
            }
            
            _textBox.sprite = profile.TextBoxSprite;
        }
    }
}
