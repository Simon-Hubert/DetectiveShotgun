using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using System;
using System.Threading;

namespace DShotgun.Dialogs
{
    public class CommentDisplayer : ATextDisplayer
    {
        [Header("data")]
        [SerializeField] private float _defaultTimeToHide = 3.0f;

        private void Awake() {
            base.Awake();
        }

        public override async Awaitable DisplayAsync(CharacterDialogData a_dialogData, CancellationToken a_cancel) {
            try {
                if (_displayState == TextDisplayState.Closed)
                    ShowCanvas = true;

                _speakersName.text = a_dialogData.Name;
                
                _displayState = TextDisplayState.Writing;
                await _dumbReader.ComputeCharByCharAsync(a_dialogData.Text, GetProfile(a_dialogData), a_cancel);

                _displayState = TextDisplayState.Idle;
                await Awaitable.WaitForSecondsAsync(_defaultTimeToHide, a_cancel);
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
    }
}
