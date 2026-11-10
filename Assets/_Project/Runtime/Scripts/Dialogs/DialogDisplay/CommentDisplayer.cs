using UnityEngine;
using NaughtyAttributes;
using System;
using System.Threading;
using System.Collections.Generic;

namespace DShotgun.Dialogs
{
    public class CommentDisplayer : ATextDisplayer
    {
        [Header("data")]
        [SerializeField] private float _defaultTimeToHide = 3.0f;
        [Space(7)]
        [SerializeField, ReadOnly] private int _currentGroupID = 0;
        public int GroupID
        {
            get => _currentGroupID;
            set {
                _currentGroupID = value;
                _listOfTextID.Clear();
            }
        }
        [SerializeField, ReadOnly] private List<CommentData> _listOfTextID = new List<CommentData>();
        [SerializeField, ReadOnly] private int _index = 0;
        
        private void Awake() {
            base.Awake();
        }

        public override async Awaitable DisplayAsync(CharacterDialogData a_dialogData, CancellationToken a_cancel) {
            try {
                if (_displayState == TextDisplayState.Closed)
                    ShowCanvas = true;
                
                for (_index = 0; _index < _listOfTextID.Count; _index++) {
                    a_dialogData = _listOfTextID[_index].dialogData;
                    _speakersName.text = a_dialogData.Name;
                
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

        public void AddNewComment(CharacterDialogData a_dialogData, int a_id) {
            foreach (CommentData var in _listOfTextID) {
                if (a_id == var.id) return;
            }
            
            _listOfTextID.Add(new CommentData(a_dialogData, a_id));
        }

        private struct CommentData {
            public int id { get; private set; }
            public CharacterDialogData dialogData { get; private set; }

            public CommentData(CharacterDialogData a_dialogData, int a_id) {
                id = a_id;
                dialogData = a_dialogData;
            }
        }
    }
}
