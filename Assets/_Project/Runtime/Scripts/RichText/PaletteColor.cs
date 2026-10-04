using System;
using UnityEngine;

namespace DShotgun
{
    [Serializable]
    public struct PaletteColor
    {
        [SerializeField] private string _name;
        [SerializeField] private Color _color;

        public string Name => _name;
        public Color Color => _color;
    }
}
