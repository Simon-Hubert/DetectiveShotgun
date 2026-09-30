using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMockup : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image _chara1;
    [SerializeField] private Image _chara2;
    [Space(7)]
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _talk;
    
    [Header("Parameters")]
    [SerializeField, Range(0, 2)] private float _scaleResize = 1.0f;
    private Vector3 _scaleChara1;
    private Vector3 _scaleChara2;
    [SerializeField, Range(0, 1)] private float _shadowFactor;
    [Space(7)]
    [SerializeField] private string _nameChara1;
    [SerializeField] private string _nameChara2;
    [Space(7)]
    [SerializeField, TextArea] private string _textChara1;
    [SerializeField, TextArea] private string _textChara2;

    [SerializeField, ReadOnly] private int _charaId = 1;
    
    
    void Start() {
        _scaleChara1 = _chara1.rectTransform.localScale;
        _scaleChara2 = _chara1.rectTransform.localScale;
        
        SwitchDialogue(_charaId);
    }

    private void SwitchDialogue(int a_charaId)
    {
        if (a_charaId == 2) {
            _chara2.color = Color.white;
            _chara1.color = Color.Lerp(Color.white, Color.black, _shadowFactor);

            _chara2.rectTransform.localScale = _scaleChara2;
            _chara1.rectTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, _scaleChara1, _scaleResize);

            _name.text = _nameChara2;
            _talk.text = _textChara2;
        }
        else {
            _chara1.color = Color.white;
            _chara2.color = Color.Lerp(Color.white, Color.black, _shadowFactor);

            _chara1.rectTransform.localScale = _scaleChara1;
            _chara2.rectTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, _scaleChara2, _scaleResize);

            _name.text = _nameChara1;
            _talk.text = _textChara1;
        }
    }
    public void SwitchDialogue() {
        if (_charaId == 1) _charaId = 2;
        else _charaId = 1;
        
        SwitchDialogue(_charaId);
    }
}
