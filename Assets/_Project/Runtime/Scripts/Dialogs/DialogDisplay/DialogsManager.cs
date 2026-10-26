using System.Threading;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace DShotgun.Dialogs
{
    public class DialogsManager : MonoBehaviour
    {
        public static DialogsManager instance { get; private set; }
        
        [SerializeField] private DialogsDisplayer _dialogsDisplayer;
        [SerializeField] private CommentDisplayer _commentDisplayer;
        
        [FormerlySerializedAs("textDisplayerState")]
        [FormerlySerializedAs("_dialogState")]
        [Space(7)]
        [SerializeField, ReadOnly] private TextDisplayState textDisplayState;
        public TextDisplayState GetTextDisplayState { get => textDisplayState; }
        private CancellationTokenSource _dialogCancellationTokenSource = new CancellationTokenSource();
        
        [SerializeField, ReadOnly, Tooltip("Cannot be waiting state")] private TextDisplayState _commentState;
        public TextDisplayState GetCommentState { get => _commentState; }
        private CancellationTokenSource _commentCancellationTokenSource = new CancellationTokenSource();

        [Space(7)]
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
            ShowDialogUI(!_dialogsDisplayer.ShowCanvas);
        }

        [Button("Comment Canvas")]
        private void Editor_CommentCanvas() {
            ShowCommentUI(!_commentDisplayer.ShowCanvas);
        }
        #endif
        
        public void ShowDialogUI(bool a_state) {
            _dialogsDisplayer.ShowCanvas = a_state;
            
            if (!a_state) {
                if (!_dialogCancellationTokenSource.IsCancellationRequested)
                    _dialogCancellationTokenSource.Cancel();
            }
        }
        public void CaptureDialogsInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Dialog Info");
            
            if (_commentState != TextDisplayState.Closed) {
                Debug.Log($"'{name}' : Comment was cancelled due to Dialog being called - Current Comment state '{_commentState.ToString()}'", gameObject);
                ShowCommentUI(false);
            }
            
            if (!_dialogCancellationTokenSource.IsCancellationRequested)
                _dialogCancellationTokenSource.Cancel();
            _dialogCancellationTokenSource = new CancellationTokenSource();
            
            _ = _dialogsDisplayer.DisplayAsync(a_dialogData, _dialogCancellationTokenSource.Token);
        }
        
        public void ShowCommentUI(bool a_state) {
            _commentDisplayer.ShowCanvas = a_state;
            
            if (!a_state) {
                if (!_commentCancellationTokenSource.IsCancellationRequested)
                    _commentCancellationTokenSource.Cancel();
            }
        }
        public void CaptureCommentInfo(CharacterDialogData a_dialogData) {
            Debug.Log("Capture Comment Info");

            if (textDisplayState != TextDisplayState.Closed) {
                Debug.LogError($"'{name}' : Comment was cancelled, Dialog state is not 'Closed' - Current Dialog state '{textDisplayState.ToString()}'", gameObject);
                return;
            }
            
            if (!_commentCancellationTokenSource.IsCancellationRequested)
                _commentCancellationTokenSource.Cancel();
            _commentCancellationTokenSource = new CancellationTokenSource();
            
            _ = _commentDisplayer.DisplayAsync(a_dialogData, _commentCancellationTokenSource.Token);
        }
    }
}
