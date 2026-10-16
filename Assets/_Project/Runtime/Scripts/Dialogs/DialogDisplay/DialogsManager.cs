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
        private CancellationTokenSource _dialogCancellationTokenSource = new CancellationTokenSource();
        
        [Space(7)]
        [SerializeField, ReadOnly, Tooltip("Cannot be waiting state")] private DialogState _commentState;
        public DialogState GetCommentState { get => _commentState; }
        private CancellationTokenSource _commentCancellationTokenSource = new CancellationTokenSource();

        [SerializeField] private float _timeToHide = 3.0f;

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
        
        #if UNITY_EDITOR
        [Button("Hide All Canvas")]
        private void Editor_HideAllCanvas() {
            ShowDialogUI(false);
            ShowCommentUI(false);
        }

        [Button("Dialog Canvas")]
        private void Editor_DialogCanvas() {
            ShowDialogUI(!_dialogsDisplayer.DisplayDialogCanvas);
        }

        [Button("Comment Canvas")]
        private void Editor_CommentCanvas() {
            ShowCommentUI(!_dialogsDisplayer.DisplayCommentCanvas);
        }
        #endif
        
        public void ShowDialogUI(bool a_state) {
            _dialogState = a_state ? DialogState.Idle : DialogState.Closed;
            _dialogsDisplayer.DisplayDialogCanvas = a_state;
            
            if (!a_state) {
                if (!_dialogCancellationTokenSource.IsCancellationRequested)
                    _dialogCancellationTokenSource.Cancel();
            }
        }
        public void CaptureDialogsInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Dialog Info");
            
            if (_commentState != DialogState.Closed) {
                Debug.Log($"'{name}' : Comment was cancelled due to Dialog being called - Current Comment state '{_commentState.ToString()}'", gameObject);
                ShowCommentUI(false);
            }
            
            if (!_dialogCancellationTokenSource.IsCancellationRequested)
                _dialogCancellationTokenSource.Cancel();
            _dialogCancellationTokenSource = new CancellationTokenSource();
            
            _ = ComputeDialogsInfoAsync(_dialogCancellationTokenSource.Token, a_dialogData);
        }
        private async Awaitable ComputeDialogsInfoAsync(CancellationToken a_cancel, CharacterDialogData a_dialogData) {
            try {
                if (_dialogState == DialogState.Closed) 
                    ShowDialogUI(true);
                
                _dialogsDisplayer.DisplayDialogSprite(a_dialogData.Left, a_dialogData.Middle, a_dialogData.Right);
                _dialogsDisplayer.DisplayDialogSpeaker(a_dialogData.Name, (CharaPos)a_dialogData.SpeakingCharacterID);

                _dialogState = DialogState.Writing;
                SOCharacterDialogProfile profile = a_dialogData.DialogProfile == null ? _defaultProfile : a_dialogData.DialogProfile;
                await _dialogsDisplayer.DisplayDialogAsync(a_cancel, profile, a_dialogData.Text);
                
                _dialogState = _dialogsDisplayer.DialogAutoContinue ? DialogState.Idle : DialogState.Waiting;
                while (_dialogState == DialogState.Waiting) {
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
                if (_dialogState != DialogState.Closed)
                    a_dialogData.Callback();
            }
        }
        
        public void ShowCommentUI(bool a_state) {
            _commentState = a_state ? DialogState.Idle : DialogState.Closed;
            _dialogsDisplayer.DisplayCommentCanvas = a_state;
            
            if (!a_state) {
                if (!_commentCancellationTokenSource.IsCancellationRequested)
                    _commentCancellationTokenSource.Cancel();
            }
        }
        public void CaptureCommentInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Comment Info");

            if (_dialogState != DialogState.Closed) {
                Debug.LogError($"'{name}' : Comment was cancelled, Dialog state is not 'Closed' - Current Dialog state '{_dialogState.ToString()}'", gameObject);
                return;
            }
            
            if (!_commentCancellationTokenSource.IsCancellationRequested)
                _commentCancellationTokenSource.Cancel();
            _commentCancellationTokenSource = new CancellationTokenSource();
            
            _ = ComputeCommentInfoAsync(_commentCancellationTokenSource.Token, a_dialogData);
        }
        private async Awaitable ComputeCommentInfoAsync(CancellationToken a_cancel, CharacterDialogData a_dialogData) {
            try {
                if (_commentState == DialogState.Closed)
                    ShowCommentUI(true);

                _commentState = DialogState.Writing;
                SOCharacterDialogProfile profile =
                    a_dialogData.DialogProfile == null ? _defaultProfile : a_dialogData.DialogProfile;
                await _dialogsDisplayer.DisplayCommentAsync(a_cancel, a_dialogData.Name, profile, a_dialogData.Text);

                _commentState = DialogState.Idle;
                await Awaitable.WaitForSecondsAsync(_timeToHide, a_cancel);
                ShowCommentUI(false);
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
