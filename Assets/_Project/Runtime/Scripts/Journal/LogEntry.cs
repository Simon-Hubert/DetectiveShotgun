using TMPro;
using UnityEngine;

namespace DShotgun
{
    public class LogEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _character;
        [SerializeField] private TextMeshProUGUI _text;


        public void Init(DialogEntryData data)
        {
            _character.text = data.CharacterName;
            _text.text = data.Text;
        }
    }
}
