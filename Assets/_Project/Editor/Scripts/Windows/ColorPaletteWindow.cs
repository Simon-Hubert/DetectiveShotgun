using System;
using UnityEditor;
using UnityEngine;

namespace DShotgun.Editor
{
    public class ColorPaletteWindow : EditorWindow
    {
        private const int COLUMNS = 6;
        private const float SWATCH_SIZE = 28f;
        private const float SPACING = 4f;
        private const float PADDING = 8f;
        private const float CUSTOM_SECTION_HEIGHT = 70f;
        private const float BASE_HEIGHT = 92f; // titre, séparateur, foldout, bouton Annuler

        private static ColorPaletteWindow _openWindow;

        private ColorPalette _palette;
        private Action<Color> _onColorPicked;
        private Color _customColor = Color.white;
        private bool _showCustom;

        public static void Open(Rect activatorRect, ColorPalette palette, Color initialColor, Action<Color> onColorPicked) 
        {
            if (_openWindow != null) _openWindow.Close();

            ColorPaletteWindow window = CreateInstance<ColorPaletteWindow>();
            window.titleContent = new GUIContent("Couleur du texte");
            window._palette = palette;
            window._onColorPicked = onColorPicked;
            window._customColor = initialColor;
            window.wantsMouseMove = true; // pour le survol des cases

            Rect screenRect = GUIUtility.GUIToScreenRect(activatorRect);
            window.ShowUtility();
            window.position = new Rect(screenRect.x, screenRect.yMax + 2f, window.Width, window.ComputeHeight());
            _openWindow = window;
        }

        private float Width => PADDING * 2f + COLUMNS * SWATCH_SIZE + (COLUMNS - 1) * SPACING;

        private int ColorCount => _palette == null ? 0 : _palette.Colors.Count;

        private float PaletteHeight 
        {
            get 
            {
                if (ColorCount == 0) return EditorGUIUtility.singleLineHeight * 2f;
                int rows = Mathf.CeilToInt(ColorCount / (float)COLUMNS);
                return rows * SWATCH_SIZE + (rows - 1) * SPACING;
            }
        }

        private float ComputeHeight() => BASE_HEIGHT + PaletteHeight + (_showCustom ? CUSTOM_SECTION_HEIGHT : 0f);

        private void OnGUI() 
        {
            if (Event.current.type == EventType.MouseMove) Repaint();

            GUILayout.Space(PADDING);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(PADDING);
                using (new GUILayout.VerticalScope()) 
                {
                    EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);
                    DrawPalette();

                    GUILayout.Space(SPACING * 2f);
                    EditorGUILayout.LabelField(GUIContent.none, GUI.skin.horizontalSlider);

                    DrawCustomColor();

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Annuler")) Close();
                }
                GUILayout.Space(PADDING);
            }
            GUILayout.Space(PADDING);
        }

        private void DrawPalette() 
        {
            if (ColorCount == 0)
            {
                string message = _palette == null
                    ? "Aucune ColorPalette dans le projet (Create > DetectiveShotgun > RichText > Color Palette)."
                    : "La palette est vide.";
                EditorGUILayout.HelpBox(message, MessageType.Info);
                return;
            }

            Rect area = GUILayoutUtility.GetRect(Width - PADDING * 2f, PaletteHeight);
            for (int i = 0; i < ColorCount; i++) 
            {
                PaletteColor entry = _palette.Colors[i];
                int row = i / COLUMNS;
                int column = i % COLUMNS;
                Rect swatch = new(
                    area.x + column * (SWATCH_SIZE + SPACING),
                    area.y + row * (SWATCH_SIZE + SPACING),
                    SWATCH_SIZE, SWATCH_SIZE);

                bool hovered = swatch.Contains(Event.current.mousePosition);
                if (hovered) EditorGUI.DrawRect(Expand(swatch, 2f), Color.white);
                EditorGUI.DrawRect(swatch, entry.Color);

                string tooltip = $"{entry.Name} (#{ColorUtility.ToHtmlStringRGB(entry.Color)})";
                if (GUI.Button(swatch, new GUIContent(string.Empty, tooltip), GUIStyle.none))
                {
                    Pick(entry.Color);
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void DrawCustomColor()
        {
            bool showCustom = EditorGUILayout.Foldout(_showCustom, "Couleur personnalisée", true);
            if (showCustom != _showCustom)
            {
                _showCustom = showCustom;
                position = new Rect(position.x, position.y, Width, ComputeHeight());
            }
            if (!_showCustom) return;
            
            _customColor = EditorGUILayout.ColorField(GUIContent.none, _customColor, true, false, false);

            if (!IsInPalette(_customColor)) 
            {
                EditorGUILayout.LabelField("Couleur hors palette", EditorStyles.miniLabel);
            }

            if (GUILayout.Button("Appliquer"))
            {
                Pick(_customColor);
                GUIUtility.ExitGUI();
            }
        }

        private bool IsInPalette(Color color)
        {
            for (int i = 0; i < ColorCount; i++) 
            {
                if (ColorUtility.ToHtmlStringRGB(_palette.Colors[i].Color) == ColorUtility.ToHtmlStringRGB(color)) return true;
            }
            return false;
        }

        private void Pick(Color color) 
        {
            _onColorPicked?.Invoke(color);
            Close();
        }
        
        private void OnSelectionChange() => Close();

        private void OnDestroy()
        {
            if (_openWindow == this) _openWindow = null;
        }

        private static Rect Expand(Rect rect, float amount) =>
            new(rect.x - amount, rect.y - amount, rect.width + amount * 2f, rect.height + amount * 2f);
    }
}
