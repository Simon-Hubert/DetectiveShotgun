using System;
using System.Collections.Generic;
using System.Threading;
using HierarchySequences;
using UnityEngine;
using UnityEngine.Events;

namespace DShotgun.Dialogs
{
    public class PausableSequencer : MonoBehaviour
    {
        [SerializeField] private DialogFrame _whenGetBack;
        [SerializeField] private UnityEvent _OnEnded;
        
        private DialogFrame[] _dialogFrames;

        private CancellationTokenSource _cancellationTokenSource;
        private int _currentIndex = 0;
        private bool _wasCancelled = false;
        
        private void Awake()
        {
            List<DialogFrame> dialogFrames = new List<DialogFrame>();
            foreach (Transform child in transform) {
                if (!child.gameObject.activeSelf) continue;
                DialogFrame sequencable = child.GetComponent<DialogFrame>();
                if(sequencable) dialogFrames.Add(sequencable);
            }
            _dialogFrames = dialogFrames.ToArray();
        }

        public void StopExecution()
        {
            _cancellationTokenSource.Cancel();
            DialogsManager.instance.ShowDialogUI(false);
        }

        public void StartExecution()
        {
            DialogsManager.instance.ShowDialogUI(true);
            _ = ExecuteAsync();
        }

        private async Awaitable ExecuteAsync()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                if (_wasCancelled)
                {
                    _wasCancelled = false;
                    _ = _whenGetBack.ExecuteAsync();
                    await Awaitable.WaitForSecondsAsync(2f, _cancellationTokenSource.Token);
                    _whenGetBack.OnRequestedSkip();
                }
                
                while (_currentIndex < _dialogFrames.Length)
                {
                    await PlayNext(_cancellationTokenSource.Token);
                }
                DialogsManager.instance.ShowDialogUI(false);
            }
            catch (OperationCanceledException oce)
            {
                _wasCancelled = true;
                Debug.Log($"Cancelled at index {_currentIndex}");
            }
            _OnEnded?.Invoke();
        }

        private async Awaitable PlayNext(CancellationToken token)
        {
            if (_currentIndex >= _dialogFrames.Length) return;
            _ = _dialogFrames[_currentIndex].ExecuteAsync();
            await Awaitable.WaitForSecondsAsync(2f, token);
            _dialogFrames[_currentIndex].OnRequestedSkip();
            _currentIndex++;
        }
    }
}
