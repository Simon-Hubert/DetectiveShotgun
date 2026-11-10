using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace DShotgun.Dialogs
{
    public class TextReader : AReader
    {
        [SerializeField] private TextMeshProUGUI _textUI;
        [SerializeField] private SOCharacterDialogProfile _dialogProfile;
        private TextDumbReader _dumbReader;
        
        private void Start()
        {
            _dumbReader = new TextDumbReader(_textUI);
        }

        public override async Awaitable ComputeTextAsync(string text, CancellationToken cancel)
        {
            await _dumbReader.ComputeTextAsync(text, _dialogProfile, cancel);
        }
    }
}
