using System;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;

namespace DShotgun.Dialogs
{
    public class DialogsManager : MonoBehaviour
    {
        public static DialogsManager instance { get; private set; }
        
        [SerializeField] private DialogsDisplaying _dialogsDisplayer;
        [SerializeField, ReadOnly] private DialogState _dialogState;
        public DialogState GetDialogState { get => _dialogState; }

        [Space(7)]
        public SOCharacterDialogProfile _defaultProfile;
        private CancellationTokenSource _tokenSource;
        
        
        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void CaptureDialogsInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Debug Info");
            _ = ComputeDialogsInfoAsync(CancellationToken.None, a_dialogData);
        }

        private async Awaitable ComputeDialogsInfoAsync(CancellationToken a_cancel, CharacterDialogData a_dialogData)
        {
            try {
                _dialogsDisplayer.DisplaySprite(a_dialogData.Left, a_dialogData.Middle, a_dialogData.Right);
                _dialogsDisplayer.DisplaySpeaker(a_dialogData.Name, (CharaPos)a_dialogData.SpeakingCharacterID);

                _dialogState = DialogState.Writing;

                SOCharacterDialogProfile profile = a_dialogData.DialogProfile == null ? _defaultProfile : a_dialogData.DialogProfile;
                await _dialogsDisplayer.DisplayTextAsync(a_cancel, profile, a_dialogData.Text);
                _dialogState = _dialogsDisplayer.AutoContinue ? DialogState.Idle : DialogState.Waiting;

                while (_dialogState == DialogState.Waiting)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Compute Dialog Async was cancelled - {e}", gameObject);
            }
            finally {
                a_dialogData.Callback();
            }
        }

        public void ContinueDialog() {
            if (_dialogState == DialogState.Waiting) {
                _dialogState = DialogState.Idle;
            }
        }
    }

    [Serializable]
    public enum DialogState {
        Closed = -1,
        Idle = 0,
        Writing = 1,
        Waiting = 2
    }
}
