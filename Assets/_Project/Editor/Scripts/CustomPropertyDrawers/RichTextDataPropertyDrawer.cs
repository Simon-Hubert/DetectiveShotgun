using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DShotgun.Editor
{
    [CustomPropertyDrawer(typeof(RichTextData))]
    public class RichTextDataPropertyDrawer : PropertyDrawer
    {
        private const float TOOLBAR_HEIGHT = 21f;   // hauteur du style EditorStyles.toolbar
        private const float TAB_WIDTH = 64f;
        private const float STYLE_BUTTON_WIDTH = 26f;
        private const float COLOR_BUTTON_WIDTH = 60f;
        private const int MIN_TEXT_LINES = 4;
        private const float INSPECTOR_MARGIN = 40f;   // largeur estimée tant que la vraie n'est pas connue
        private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

        // TextEditor interne d'EditorGUI.TextArea : GetStateObject ne le renvoie pas.
        private static readonly FieldInfo ACTIVE_EDITOR_FIELD =
            typeof(EditorGUI).GetField("activeEditor", BindingFlags.NonPublic | BindingFlags.Static);

        // Sélection mémorisée : la zone de texte perd le focus au clic sur un bouton.
        private static string _selectionPath;
        private static int _selectionStart;
        private static int _selectionLength;

        // Un PropertyDrawer est recréé et partagé par Unity : l'état de chaque champ vit ici.
        private class FieldState
        {
            public bool IsPreview;
            public float Width;             // GetPropertyHeight ne reçoit pas la largeur

            // Cache : évite de tout recalculer à chaque événement GUI
            public uint HeightHash;
            public float HeightWidth;
            public bool HeightIsPreview;
            public float ContentHeight;
            public uint StyledTextHash;
            public string StyledText;
        }
        private static readonly Dictionary<string, FieldState> _stateByField = new();

        private static GUIStyle _boldButtonStyle;
        private static GUIStyle _italicButtonStyle;
        private static GUIStyle _textAreaStyle;
        private static GUIStyle _previewStyle;
        private static GUIStyle _placeholderStyle;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitStyles();
            FieldState state = GetState(property);
            float width = state.Width > 1f ? state.Width : EditorGUIUtility.currentViewWidth - INSPECTOR_MARGIN;

            return LINE_HEIGHT + TOOLBAR_HEIGHT + GetContentHeight(property, state, width);
        }

        // Le conteneur IMGUI par défaut de l'inspecteur ne suit pas les changements de hauteur : celui-ci si.
        public override UnityEngine.UIElements.VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedObject serializedObject = property.serializedObject;
            string propertyPath = property.propertyPath;
            GUIContent label = new GUIContent(property.displayName, property.tooltip);

            UnityEngine.UIElements.IMGUIContainer container = null;
            container = new UnityEngine.UIElements.IMGUIContainer(() =>
            {
                if (serializedObject.targetObject == null) return;

                serializedObject.UpdateIfRequiredOrScript(); // relis l'obj que si il a vrmt changé
                SerializedProperty current = serializedObject.FindProperty(propertyPath);
                if (current == null) return;

                float width = container.contentRect.width;
                if (width > 1f) GetState(current).Width = width;

                Rect rect = GUILayoutUtility.GetRect(0f, GetPropertyHeight(current, label), GUILayout.ExpandWidth(true));
                OnGUI(rect, current, label);

                serializedObject.ApplyModifiedProperties();
            });
            return container;
        }

        private static string GetFieldKey(SerializedProperty property) =>
            property.serializedObject.targetObject.GetInstanceID() + "/" + property.propertyPath;

        private static FieldState GetState(SerializedProperty property)
        {
            string fieldKey = GetFieldKey(property);
            if (!_stateByField.TryGetValue(fieldKey, out FieldState state))
            {
                state = new FieldState();
                _stateByField[fieldKey] = state;
            }
            return state;
        }

        private static float GetContentHeight(SerializedProperty property, FieldState state, float width)
        {
            uint contentHash = property.contentHash;
            if (state.ContentHeight > 0f && state.HeightHash == contentHash
                && state.HeightIsPreview == state.IsPreview && Mathf.Approximately(state.HeightWidth, width))
            {
                return state.ContentHeight;
            }

            float textHeight;
            if (state.IsPreview)
            {
                textHeight = _previewStyle.CalcHeight(new GUIContent(GetStyledText(property, state)), width);
            }
            else
            {
                string rawText = property.FindPropertyRelative("_text").stringValue;
                if (rawText.EndsWith("\n")) rawText += " ";   // CalcHeight ignore une ligne vide finale
                textHeight = _textAreaStyle.CalcHeight(new GUIContent(rawText), width);
            }

            state.HeightHash = contentHash;
            state.HeightWidth = width;
            state.HeightIsPreview = state.IsPreview;
            state.ContentHeight = Mathf.Max(LINE_HEIGHT * MIN_TEXT_LINES, textHeight);
            return state.ContentHeight;
        }

        // formatage du texte, updaté que si ça change vrmt (boxedValue + Format coûtent cher)
        private static string GetStyledText(SerializedProperty property, FieldState state)
        {
            uint contentHash = property.contentHash;
            if (state.StyledText != null && state.StyledTextHash == contentHash) return state.StyledText;

            state.StyledText = RichTextFormatter.Format(property.boxedValue as RichTextData);
            state.StyledTextHash = contentHash;
            return state.StyledText;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitStyles();

            SerializedProperty text = property.FindPropertyRelative("_text");
            SerializedProperty styles = property.FindPropertyRelative("_styles");
            string controlName = "RichText_" + property.propertyPath;
            FieldState state = GetState(property);
            bool isPreview = state.IsPreview;

            if (Event.current.type == EventType.Repaint && position.width > 1f)   // largeur fausse hors Repaint
            {
                state.Width = position.width;
            }

            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;
            EditorGUI.LabelField(new Rect(position.x, y, position.width, LINE_HEIGHT), label);
            y += LINE_HEIGHT;

            Rect toolbarRect = new Rect(position.x, y, position.width, TOOLBAR_HEIGHT);
            if (Event.current.type == EventType.Repaint) EditorStyles.toolbar.Draw(toolbarRect, false, false, false, false);

            bool writeTab = GUI.Toggle(new Rect(position.x, y, TAB_WIDTH, TOOLBAR_HEIGHT), !isPreview, "Écrire", EditorStyles.toolbarButton);
            bool previewTab = GUI.Toggle(new Rect(position.x + TAB_WIDTH, y, TAB_WIDTH, TOOLBAR_HEIGHT), isPreview,
                "Aperçu", EditorStyles.toolbarButton);

            if (isPreview && writeTab)
            {
                isPreview = false;
                state.IsPreview = false;
            }
            else if (!isPreview && previewTab)
            {
                isPreview = true;
                state.IsPreview = true;
                GUI.FocusControl(null);   // pas de saisie clavier en aperçu
            }

            using (new EditorGUI.DisabledScope(isPreview))
            {
                DrawStyleButtons(position, y, property, styles, controlName);
            }

            y += TOOLBAR_HEIGHT;
            Rect contentRect = new Rect(position.x, y, position.width, position.yMax - y);

            if (isPreview)
            {
                DrawStyledTextPreview(contentRect, GetStyledText(property, state));
            }
            else
            {
                GUI.SetNextControlName(controlName);

                // Réécrire le texte à chaque événement le marque modifié : Apply en boucle → lag
                EditorGUI.BeginChangeCheck();
                string newText = EditorGUI.TextArea(contentRect, text.stringValue, _textAreaStyle);
                if (EditorGUI.EndChangeCheck()) text.stringValue = newText;

                if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() == controlName
                    && ACTIVE_EDITOR_FIELD?.GetValue(null) is TextEditor te && te.hasSelection)
                {
                    _selectionPath = property.propertyPath;
                    _selectionStart = Mathf.Min(te.cursorIndex, te.selectIndex);
                    _selectionLength = Mathf.Abs(te.cursorIndex - te.selectIndex);
                }
            }

            EditorGUI.EndProperty();
        }

        private static void DrawStyleButtons(Rect position, float y, SerializedProperty property, SerializedProperty styles, string controlName)
        {
            float x = position.xMax - (STYLE_BUTTON_WIDTH * 2f + COLOR_BUTTON_WIDTH);

            if (GUI.Button(new Rect(x, y, STYLE_BUTTON_WIDTH, TOOLBAR_HEIGHT), new GUIContent("G", "Gras"), _boldButtonStyle)
                && TryGetSelection(property, out int start, out int length))
            {
                ApplyStyle(styles, start, length, bold: true);
                GUI.FocusControl(controlName);
            }
            x += STYLE_BUTTON_WIDTH;

            if (GUI.Button(new Rect(x, y, STYLE_BUTTON_WIDTH, TOOLBAR_HEIGHT), new GUIContent("I", "Italique"), _italicButtonStyle)
                && TryGetSelection(property, out start, out length))
            {
                ApplyStyle(styles, start, length, italic: true);
                GUI.FocusControl(controlName);
            }
            x += STYLE_BUTTON_WIDTH;

            Rect colorButtonRect = new Rect(x, y, COLOR_BUTTON_WIDTH, TOOLBAR_HEIGHT);
            if (GUI.Button(colorButtonRect, "Couleur", EditorStyles.toolbarButton)
                && TryGetSelection(property, out start, out length))
            {
                OpenColorWindow(colorButtonRect, property, start, length);
            }
        }

        // EditorStyles n'est pas prêt à l'initialisation statique
        private static void InitStyles()
        {
            if (_previewStyle != null) return;

            _boldButtonStyle = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };
            _italicButtonStyle = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Italic };
            _textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
            _previewStyle = new GUIStyle(EditorStyles.textArea) { richText = true, wordWrap = true };
            _placeholderStyle = new GUIStyle(_previewStyle) { fontStyle = FontStyle.Italic };
            _placeholderStyle.normal.textColor = Color.gray;
        }

        // Sélection de CE champ, utilisable une seule fois
        private static bool TryGetSelection(SerializedProperty property, out int start, out int length)
        {
            start = _selectionStart;
            length = _selectionLength;
            bool valid = _selectionPath == property.propertyPath && length > 0;
            _selectionPath = null;
            return valid;
        }

        // La couleur arrive plus tard, quand `property` n'est plus valide : on repasse par un SerializedObject neuf
        private static void OpenColorWindow(Rect buttonRect, SerializedProperty property, int start, int length)
        {
            Object[] targets = property.serializedObject.targetObjects;
            string stylesPath = property.propertyPath + "._styles";
            Color initialColor = GetCurrentColor(property.FindPropertyRelative("_styles"), start, length);

            ColorPaletteWindow.Open(buttonRect, FindPalette(), initialColor, color =>
            {
                if (targets == null || targets.Length == 0 || targets[0] == null) return;

                using SerializedObject serializedObject = new SerializedObject(targets);
                serializedObject.Update();
                SerializedProperty styles = serializedObject.FindProperty(stylesPath);
                if (styles == null) return;

                ApplyStyle(styles, start, length, colorHex: GetColorHexFromColor(color));
                serializedObject.ApplyModifiedProperties();
            });
        }

        private static Color GetCurrentColor(SerializedProperty styles, int start, int length)
        {
            SerializedProperty style = FindStyle(styles, start, length);
            string hex = style?.FindPropertyRelative("_colorHex").stringValue;
            return !string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out Color color) ? color : Color.white;
        }

        private static ColorPalette FindPalette()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ColorPalette)}");
            return guids.Length == 0 ? null : AssetDatabase.LoadAssetAtPath<ColorPalette>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        // "#RRGGBBAA" : le format de <color=…> (TMP) et de TryParseHtmlString
        private static string GetColorHexFromColor(Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGBA(color);
        }

        private static void ApplyStyle(SerializedProperty styles, int startIndex, int length,
            bool bold = false, bool italic = false, string colorHex = null)
        {
            if (length <= 0) return;

            SerializedProperty style = FindStyle(styles, startIndex, length);
            if (style == null)
            {
                styles.arraySize++;   // recopie le dernier élément : tout est remis à zéro ensuite
                style = styles.GetArrayElementAtIndex(styles.arraySize - 1);
                style.FindPropertyRelative("_startIndex").intValue = startIndex;
                style.FindPropertyRelative("_length").intValue = length;
                style.FindPropertyRelative("_isBold").boolValue = false;
                style.FindPropertyRelative("_isItalic").boolValue = false;
                style.FindPropertyRelative("_colorHex").stringValue = "";
            }

            if (bold) style.FindPropertyRelative("_isBold").boolValue = true;
            if (italic) style.FindPropertyRelative("_isItalic").boolValue = true;
            if (!string.IsNullOrEmpty(colorHex)) style.FindPropertyRelative("_colorHex").stringValue = colorHex;
        }

        private static SerializedProperty FindStyle(SerializedProperty styles, int startIndex, int length)
        {
            for (int i = 0; i < styles.arraySize; i++)
            {
                SerializedProperty style = styles.GetArrayElementAtIndex(i);
                if (style.FindPropertyRelative("_startIndex").intValue == startIndex
                    && style.FindPropertyRelative("_length").intValue == length)
                    return style;
            }
            return null;
        }

        private static void DrawStyledTextPreview(Rect rect, string styledText)
        {
            if (string.IsNullOrEmpty(styledText))
            {
                GUI.Label(rect, "Rien à prévisualiser.", _placeholderStyle);
                return;
            }
            GUI.Label(rect, styledText, _previewStyle);
        }
    }
}
