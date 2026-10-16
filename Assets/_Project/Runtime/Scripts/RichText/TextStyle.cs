using System;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public class TextStyle
    {
        [SerializeField] private int _startIndex;
        [SerializeField] private int _length;
        [SerializeField] private bool _isBold;
        [SerializeField] private bool _isItalic;
        [SerializeField] private string _colorHex = "";

        public int StartIndex { get => _startIndex; set => _startIndex = value; }
        public int Length { get => _length; set => _length = value; }
        public bool IsBold { get => _isBold; set => _isBold = value; }
        public bool IsItalic { get => _isItalic; set => _isItalic = value; }
        public string ColorHex { get => _colorHex; set => _colorHex = value; }

        public TextStyle(int stardIndex, int lenght)
        {
            _startIndex = stardIndex;
            _length = lenght;
        }

        public TextStyle(int startIndex, int length, bool bold, bool italic, string colorHex)
        {
            _startIndex = startIndex;
            _length = length;
            _isBold = bold;
            _isItalic = italic;
            _colorHex = colorHex;
        }

        public TextStyle()
        {
            _startIndex = 0;
            _length = 0;
            _isBold = false;
            _isItalic = false;
            _colorHex = "";
        }
    }
}