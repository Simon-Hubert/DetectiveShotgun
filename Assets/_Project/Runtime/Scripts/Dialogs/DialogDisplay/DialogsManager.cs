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
        
        [SerializeField, ReadOnly] private CommentState _commentState;
        public CommentState GetCommentState { get => _commentState; }

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

        private void Start() {
            ShowDialogUI(false);
            ShowCommentUI(false);
        }

        public void ShowDialogUI(bool a_state) {
            _dialogState = a_state ? DialogState.Idle : DialogState.Closed;
            _dialogsDisplayer.DisplayDialogCanvas(a_state);
        }
        public void CaptureDialogsInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Dialog Info");
            _ = ComputeDialogsInfoAsync(CancellationToken.None, a_dialogData);
        }
        private async Awaitable ComputeDialogsInfoAsync(CancellationToken a_cancel, CharacterDialogData a_dialogData)
        {
            try
            {
                if (_dialogState == DialogState.Closed) 
                    ShowDialogUI(true);
                
                _dialogsDisplayer.DisplayDialogSprite(a_dialogData.Left, a_dialogData.Middle, a_dialogData.Right);
                _dialogsDisplayer.DisplayDialogSpeaker(a_dialogData.Name, (CharaPos)a_dialogData.SpeakingCharacterID);

                _dialogState = DialogState.Writing;
                SOCharacterDialogProfile profile = a_dialogData.DialogProfile == null ? _defaultProfile : a_dialogData.DialogProfile;
                await _dialogsDisplayer.DisplayDialogAsync(a_cancel, profile, a_dialogData.Text);
                
                _dialogState = _dialogsDisplayer.DialogAutoContinue ? DialogState.Idle : DialogState.Waiting;
                while (_dialogState == DialogState.Waiting)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Compute Dialog Async was cancelled - {e}", gameObject);
            }
            finally {
                if (_dialogState != DialogState.Closed)
                    a_dialogData.Callback();
            }
        }

        public void ShowCommentUI(bool a_state) {
            _commentState = a_state ? CommentState.Idle : CommentState.Closed;
            _dialogsDisplayer.DisplayCommentCanvas(a_state);
        }
        public void CaptureCommentInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Comment Info");
            ComputeCommentInfoAsync(CancellationToken.None, a_dialogData);
        }
        private async Awaitable ComputeCommentInfoAsync(CancellationToken a_cancel, CharacterDialogData a_dialogData)
        {
            try {
                if (_commentState == CommentState.Closed) 
                    ShowCommentUI(true);
                
                _commentState = CommentState.Writing;
                SOCharacterDialogProfile profile = a_dialogData.DialogProfile == null ? _defaultProfile : a_dialogData.DialogProfile;
                await _dialogsDisplayer.DisplayCommentAsync(a_cancel, a_dialogData.Name, profile, a_dialogData.Text);
                
                _commentState = CommentState.Idle;
                await Awaitable.WaitForSecondsAsync(3.0f, a_cancel);
            }
            catch (Exception e) {
                Debug.LogWarning($"'{name}' : Compute Dialog Async was cancelled - {e}", gameObject);
            }
            finally {
                ShowCommentUI(false);
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

    public enum CommentState {
        Closed = -1,
        Idle = 0,
        Writing = 1
    }
}
