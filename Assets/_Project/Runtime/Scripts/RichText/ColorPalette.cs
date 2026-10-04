using System.Collections.Generic;
using UnityEngine;

namespace DShotgun
{
    [CreateAssetMenu(menuName = "DetectiveShotgun/RichText/Color Palette")]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField] private List<PaletteColor> _colors = new();

        public IReadOnlyList<PaletteColor> Colors => _colors;
    }
}
