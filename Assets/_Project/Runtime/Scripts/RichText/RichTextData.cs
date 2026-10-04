using System;
using System.Collections.Generic;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public class RichTextData
    {
        [SerializeField] private string _text = "";
        [SerializeField] private List<TextStyle> _styles = new();
        
        public string Text { get => _text; set => _text = value; }
        public List<TextStyle> Styles { get => _styles; }
        
        public void AddStyle(TextStyle style) { _styles.Add(style); }
        public void ClearStyles() { _styles.Clear(); }
    }
}
