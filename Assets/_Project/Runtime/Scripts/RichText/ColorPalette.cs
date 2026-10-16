using System.Collections.Generic;
using System;
using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(menuName = "DetectiveShotgun/RichText/Color Palette")]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField] private List<PaletteColor> _colors = new();

        public IReadOnlyList<PaletteColor> Colors => _colors;
    }

    [Serializable]
    public struct PaletteColor
    {
        [SerializeField] private string _name;
        [SerializeField] private Color _color;

        public string Name => _name;
        public Color Color => _color;
    }
}
