using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DShotgun.Dialogs
{
    public class DialogsDisplaying : MonoBehaviour
    {
        [Header("Characters Display")]
        [SerializeField] private Image _leftCharaRdr;
        private Vector3 _leftCharaPos;
        [SerializeField] private Image _middleCharaRdr;
        private Vector3 _middleCharaPos;
        [SerializeField] private Image _rightCharaRdr;
        private Vector3 _rightCharaPos;

        [Header("TextBox Display")]
        [SerializeField] private Image _textBox;
        [SerializeField] private TextMeshProUGUI _speakersName;
        [SerializeField] private TextMeshProUGUI _dialog;
        [SerializeField] private Image _continueButton;

        public void DisplayText(string a_text)
        {
            _ = DisplayTextAsync(CancellationToken.None, a_text);
        }

        private async Awaitable DisplayTextAsync(CancellationToken cancel, string a_text) {
            try {
                
            }
            catch (OperationCanceledException) {
                Debug.LogWarning($"'{name}' : Display Text Async was cancelled", gameObject);
            }
        }
    }
}
